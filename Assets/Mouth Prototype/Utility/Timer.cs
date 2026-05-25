using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace Mouth_Prototype.Utility
{
    // ─────────────────────────────────────────────────────────────
    //  TimerBehavior
    //  MonoBehaviour wrapper — attach to a GameObject when you need
    //  a Timer driven by Unity's update loop (DELTA mode) or need
    //  coroutine support. For code-only usage (e.g. inside a
    //  FoodEffect) just new up a Timer directly.
    // ─────────────────────────────────────────────────────────────
    public class TimerBehavior : MonoBehaviour
    {
        [field: SerializeField] private float duration;
        [field: SerializeField] private TimerType timerType = TimerType.DELTA;
        [field: SerializeField] private bool playOnStart = false;

        [field: SerializeField] private UnityEvent onTimerStart  = null;
        [field: SerializeField] private UnityEvent onTimerEnd    = null;
        [field: SerializeField] private UnityEvent onTimerPaused = null;
        [field: SerializeField] private UnityEvent<float> onTimerTick = null; // normalized 0→1

        public Timer Timer { get; private set; }

        private void Awake()
        {
            Timer = new Timer(duration, timerType);
            Timer.OnTimerStart  += () => onTimerStart?.Invoke();
            Timer.OnTimerEnd    += () => onTimerEnd?.Invoke();
            Timer.OnTimerPaused += () => onTimerPaused?.Invoke();
            Timer.OnTimerTick   += t  => onTimerTick?.Invoke(t);
        }

        private void Start()
        {
            if (playOnStart)
                Timer.Start(this);
        }

        // DELTA timers must be ticked manually each frame.
        private void Update()
        {
            if (timerType == TimerType.DELTA)
                Timer.Tick(Time.deltaTime);
        }
    }


    // ─────────────────────────────────────────────────────────────
    //  Timer  (plain C# — no MonoBehaviour)
    //
    //  Three modes:
    //    DELTA     — you call Tick(deltaTime) each frame yourself
    //                (or let TimerBehavior do it). Lightest weight;
    //                good for effect durations inside FoodEffect.
    //
    //    COROUTINE — needs a MonoBehaviour runner passed to Start().
    //                Respects Time.timeScale by default; pass
    //                unscaled:true to ignore it.
    //
    //    ASYNC     — fire-and-forget Task. No runner needed.
    //                Uses Task.Yield so it follows unscaled time.
    //
    //  Controls:  Start / Stop / Pause / Resume / Reset
    // ─────────────────────────────────────────────────────────────
    public class Timer
    {
        // ── State ────────────────────────────────────────────────
        public TimerType Type           { get; }
        public float     Duration       { get; private set; }
        public float     RemainingSeconds { get; private set; }
        public bool      IsRunning      { get; private set; }
        public bool      IsPaused       { get; private set; }

        /// Normalized progress 0 (just started) → 1 (complete).
        public float Progress => Duration > 0f
            ? 1f - Mathf.Clamp01(RemainingSeconds / Duration)
            : 1f;

        // ── Events ───────────────────────────────────────────────
        public event Action        OnTimerStart;
        public event Action        OnTimerEnd;
        public event Action        OnTimerPaused;
        public event Action        OnTimerResumed;
        /// Fires every tick with normalized progress (0→1).
        public event Action<float> OnTimerTick;

        // ── Internal ─────────────────────────────────────────────
        private Coroutine _coroutine;
        private MonoBehaviour _runner; // only used by COROUTINE mode

        // ── Constructors ─────────────────────────────────────────
        public Timer(float duration, TimerType type = TimerType.DELTA)
        {
            Duration         = duration;
            RemainingSeconds = duration;
            Type             = type;
        }

        // ── Controls ─────────────────────────────────────────────

        /// Start (or restart) the timer.
        /// <param name="runner">Required for COROUTINE mode; ignored otherwise.</param>
        /// <param name="unscaled">COROUTINE only — use unscaled time.</param>
        public void Start(MonoBehaviour runner = null, bool unscaled = false)
        {
            Stop();
            RemainingSeconds = Duration;
            IsRunning        = true;
            IsPaused         = false;

            switch (Type)
            {
                case TimerType.DELTA:
                    OnTimerStart?.Invoke();
                    break;

                case TimerType.COROUTINE:
                    _runner    = runner;
                    _coroutine = runner.StartCoroutine(TickCoroutine(unscaled));
                    break;

                case TimerType.ASYNC:
                    _ = TickAsync();
                    break;
            }
        }

        /// Pause a running timer (DELTA and COROUTINE only).
        public void Pause()
        {
            if (!IsRunning || IsPaused) return;
            IsPaused = true;

            if (Type == TimerType.COROUTINE && _coroutine != null && _runner != null)
                _runner.StopCoroutine(_coroutine);

            OnTimerPaused?.Invoke();
        }

        /// Resume a paused timer.
        public void Resume(bool unscaled = false)
        {
            if (!IsPaused) return;
            IsPaused = false;

            if (Type == TimerType.COROUTINE && _runner != null)
                _coroutine = _runner.StartCoroutine(TickCoroutine(unscaled));

            OnTimerResumed?.Invoke();
        }

        /// Stop and reset remaining time to full duration.
        public void Stop()
        {
            if (Type == TimerType.COROUTINE && _coroutine != null && _runner != null)
                _runner.StopCoroutine(_coroutine);

            IsRunning        = false;
            IsPaused         = false;
            _coroutine       = null;
        }

        /// Reset remaining time without stopping.
        public void Reset()
        {
            RemainingSeconds = Duration;
        }

        /// Change duration at runtime (e.g. cooldown reduction).
        public void SetDuration(float newDuration)
        {
            Duration = Mathf.Max(0f, newDuration);
            if (RemainingSeconds > Duration)
                RemainingSeconds = Duration;
        }

        // ── DELTA tick — call from Update / FixedUpdate ───────────
        public void Tick(float deltaTime)
        {
            if (Type != TimerType.DELTA) return;
            if (!IsRunning || IsPaused)  return;
            if (RemainingSeconds <= 0f)  return;

            RemainingSeconds = Mathf.Max(0f, RemainingSeconds - deltaTime);
            OnTimerTick?.Invoke(Progress);

            if (RemainingSeconds <= 0f)
                Complete();
        }

        // ── COROUTINE tick ────────────────────────────────────────
        private IEnumerator TickCoroutine(bool unscaled)
        {
            OnTimerStart?.Invoke();

            while (RemainingSeconds > 0f)
            {
                yield return null;
                if (IsPaused) yield break; // paused — coroutine will restart on Resume

                float delta = unscaled ? Time.unscaledDeltaTime : Time.deltaTime;
                RemainingSeconds = Mathf.Max(0f, RemainingSeconds - delta);
                OnTimerTick?.Invoke(Progress);
            }

            Complete();
        }

        // ── ASYNC tick ────────────────────────────────────────────
        private async Task TickAsync()
        {
            OnTimerStart?.Invoke();

            while (RemainingSeconds > 0f)
            {
                await Task.Yield();
                if (!IsRunning) return; // stopped externally

                // Async always uses unscaled time — no Time.deltaTime in non-main threads.
                RemainingSeconds = Mathf.Max(0f, RemainingSeconds - Time.unscaledDeltaTime);
                OnTimerTick?.Invoke(Progress);
            }

            Complete();
        }

        // ── Shared completion ─────────────────────────────────────
        private void Complete()
        {
            RemainingSeconds = 0f;
            IsRunning        = false;
            IsPaused         = false;
            _coroutine       = null;
            OnTimerEnd?.Invoke();
        }
    }

    public enum TimerType { DELTA, COROUTINE, ASYNC }
}