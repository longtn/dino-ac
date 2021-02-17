using AC.Base;
using System;
using System.Threading;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace AC.Pages
{
    public class SettingsPageViewModel : ViewModelBase
    {
        const int MOUSEEVENTF_LEFTDOWN = 0x0002; /* left button down */
        const int MOUSEEVENTF_LEFTUP = 0x0004; /* left button up */

        [DllImport("user32")]
        public static extern int SetCursorPos(int x, int y);

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);

        private int _currentX, _currentY;
        private int _inputX, _inputY, _inputX2, _inputY2;
        private int _inputTime;
        private double _countTime;
        private string _runningState = "Start";
        private bool _isAuto = false;

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

        public double CountTime
        {
            get => _countTime;
            set => SetPropertyValue(ref _countTime, value);
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

        public System.Windows.Input.ICommand AutoClickCommand => new RelayCommand(OnAutoClick);

        public SettingsPageViewModel(SettingsPage view) : base(view)
        {
            this._inputTime = 1;

            Thread threadCheckPos = new Thread(new ThreadStart(this.ThreadCheckPos));
            threadCheckPos.IsBackground = true;
            threadCheckPos.Start();

            Thread threadAuto = new Thread(new ThreadStart(this.ThreadAuto));
            threadAuto.IsBackground = true;
            threadAuto.Start();
        }

        private void OnAutoClick()
        {
            IsAuto = !IsAuto;

            if (this._isAuto)
                RunningState = "Running (F5)";
            else
                RunningState = "Start (F5)";
        }

        private void ThreadCheckPos()
        {
            while (true)
            {
                CurrentX = Cursor.Position.X;
                CurrentY = Cursor.Position.Y;

                var current = UnixTimeNow();
                var val = UnixTimeNow() - this._min;
                var total = this._max - this._min;
                CountTime = 100.0 * val / total;

                Thread.Sleep(100);
            }
        }

        private void ThreadAuto()
        {
            while (true)
            {
                if (this._isAuto)
                {
                    var lastX = CurrentX;
                    var lastY = CurrentY;
                    var x1 = this._inputX;
                    var y1 = this._inputY;
                    var x2 = this._inputX2;
                    var y2 = this._inputY2;

                    // move mouse 1
                    SetCursorPos(x1, y1);
                    Thread.Sleep(50);

                    // click mouse 1
                    mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
                    mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
                    Thread.Sleep(100);

                    // move mouse 2
                    SetCursorPos(x2, y2);
                    Thread.Sleep(50);

                    // click mouse 2
                    mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
                    mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
                    Thread.Sleep(100);

                    // return mouse
                    SetCursorPos(lastX, lastY);

                    var endTime = 1000 * 60 * InputTime;
                    this._min = UnixTimeNow();
                    this._max = this._min + endTime;
                    Thread.Sleep(endTime);
                }
                else
                {
                    var endTime = 1000;
                    this._min = UnixTimeNow();
                    this._max = this._min + endTime;
                    Thread.Sleep(endTime);
                }
            }
        }

        private long _min;
        private long _max;
        private long UnixTimeNow()
        {
            var timeSpan = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0));
            return (long)timeSpan.TotalMilliseconds;
        }
    }
}
