using AC.Base;
using System;
using System.Threading;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace AC.Pages
{
    enum ClickType
    {
        _121 = 0,
        _12 = 1,
        _1 = 2,
    }

    public class SettingsPageViewModel : ViewModelBase
    {
        #region declaration
        const int SLEEPTIME = 100;
        const int SEC = 1000;
        const string START = "Start (F5)";
        const string RUNNING = "Stop (F5)";

        private int _inputTime = 1;
        private int _inputX, _inputY, _inputX2, _inputY2;
        private int _currentX, _currentY;
        private double _currentTime;
        private long _minTime;
        private long _maxTime;

        private string _runningState = START;
        private bool _isAuto = false;
        private ClickType _clickType = ClickType._121;

        public int InputX
        {
            get => _inputX;
            set
            {
                SetPropertyValue(ref _inputX, value);
                InputX2 = value;
            }
        }

        public int InputY
        {
            get => _inputY;
            set
            {
                SetPropertyValue(ref _inputY, value);
                InputY2 = value;
            }
        }

        public int InputX2
        {
            get => _inputX2;
            set => SetPropertyValue(ref _inputX2, value);
        }

        public int InputY2
        {
            get => _inputY2;
            set => SetPropertyValue(ref _inputY2, value);
        }

        public int InputTime
        {
            get => _inputTime;
            set => SetPropertyValue(ref _inputTime, value);
        }

        public int CurrentX
        {
            get => _currentX;
            set => SetPropertyValue(ref _currentX, value);
        }

        public int CurrentY
        {
            get => _currentY;
            set => SetPropertyValue(ref _currentY, value);
        }

        public double CurrentTime
        {
            get => _currentTime;
            set => SetPropertyValue(ref _currentTime, value);
        }

        public string RunningState
        {
            get => this._runningState;
            set => SetPropertyValue(ref _runningState, value);
        }

        public bool IsAuto
        {
            get => this._isAuto;
            set => SetPropertyValue(ref _isAuto, value);
        }
        #endregion

        #region constructor
        public SettingsPageViewModel(SettingsPage view) : base(view)
        {
            Thread sideThread = new Thread(new ThreadStart(this.SideTask));
            sideThread.IsBackground = true;
            sideThread.Start();

            Thread mainThread = new Thread(new ThreadStart(this.MainTask));
            mainThread.IsBackground = true;
            mainThread.Start();
        }
        #endregion

        #region commands
        public System.Windows.Input.ICommand AutoClickCommand => new RelayCommand(OnAutoClick);
        public System.Windows.Input.ICommand SetInputCommand => new RelayCommand(OnSetInputCommand);
        public System.Windows.Input.ICommand SetInput2Command => new RelayCommand(OnSetInput2Command);
        public System.Windows.Input.ICommand ChangeClickTypeCommand => new ParamRelayCommand(param => OnChangeClickTypeCommand((int)param));

        private void OnAutoClick()
        {
            IsAuto = !IsAuto;

            if (this._isAuto)
                RunningState = RUNNING;
            else
                RunningState = START;
        }

        private void OnSetInputCommand()
        {
            InputX = CurrentX;
            InputY = CurrentY;
        }

        private void OnSetInput2Command()
        {
            InputX2 = CurrentX;
            InputY2 = CurrentY;
        }

        private void OnChangeClickTypeCommand(int type)
        {
            this._clickType = (ClickType)type;
        }
        #endregion

        #region main thread
        private void MainTask()
        {
            while (true)
            {
                Thread.Sleep(SLEEPTIME);

                if (!this._isAuto)
                    continue;

                var current = UnixTimeNow();
                var endTime = current + (InputTime * SEC * 60);

                if (UpdateMinMax(current, endTime))
                    DoMouseEvents();
            }
        }
        #endregion

        #region side thread
        private void SideTask()
        {
            while (true)
            {
                Thread.Sleep(SLEEPTIME);

                var current = UnixTimeNow();
                var endTime = current + SEC;

                if (!this._isAuto)
                    UpdateMinMax(current, endTime);

                var val = current - this._minTime;
                var total = this._maxTime - this._minTime;

                CurrentTime = 100.0 * val / total;
                CurrentX = Cursor.Position.X;
                CurrentY = Cursor.Position.Y;
            }
        }
        #endregion

        #region funcs
        private void DoMouseEvents()
        {
            var lastX = CurrentX;
            var lastY = CurrentY;
            var x1 = this._inputX;
            var y1 = this._inputY;
            var x2 = this._inputX2;
            var y2 = this._inputY2;

            MoveAndClickMouse(x1, y1);

            if (this._clickType != ClickType._1)
            {
                // ClickType._12
                MoveAndClickMouse(x2, y2);

                if (this._clickType == ClickType._121)
                    MoveAndClickMouse(x1, y1);
            }

            // return mouse
            MoveAndClickMouse(lastX, lastY, false);
        }

        private bool UpdateMinMax(long min, long max)
        {
            if (this._maxTime < min || max < this._maxTime)
            {
                this._minTime = min;
                this._maxTime = max;
                return true;
            }
            return false;
        }

        private long UnixTimeNow()
        {
            var timeSpan = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0));
            return (long)timeSpan.TotalMilliseconds;
        }
        #endregion

        #region mouse event
        const int MOUSEEVENTF_LEFTDOWN = 0x0002; /* left button down */
        const int MOUSEEVENTF_LEFTUP = 0x0004; /* left button up */

        [DllImport("user32")]
        public static extern int SetCursorPos(int x, int y);

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);

        private void MoveAndClickMouse(int x, int y, bool click = true)
        {
            SetCursorPos(x, y);
            Thread.Sleep(SLEEPTIME);

            if (click)
            {
                mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
                mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
                Thread.Sleep(SLEEPTIME);
            }
        }
        #endregion
    }
}
