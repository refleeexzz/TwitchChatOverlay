using System;
using System.Runtime.InteropServices;

namespace TwitchChatOverlay;

public static class WindowHelper
{
    [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr")]
    private static extern IntPtr SetWindowLongPtr(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

    [DllImport("user32.dll", EntryPoint = "GetWindowLongPtr")]
    private static extern IntPtr GetWindowLongPtr(IntPtr hWnd, int nIndex);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

    public const int GWL_EXSTYLE = -20;
    public const int WS_EX_TRANSPARENT = 0x20;
    public const int WS_EX_LAYERED = 0x80000;
    public const int WS_EX_TOOLWINDOW = 0x80; // prevents window from appearing in screen captures

    private static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
    private const uint SWP_NOSIZE = 0x0001;
    private const uint SWP_NOMOVE = 0x0002;
    private const uint SWP_SHOWWINDOW = 0x0040;

    // enables click-through by setting transparent extended style
    public static void SetWindowClickThrough(IntPtr hwnd)
    {
        var extendedStyle = GetWindowLongPtr(hwnd, GWL_EXSTYLE);
        SetWindowLongPtr(hwnd, GWL_EXSTYLE, (IntPtr)(extendedStyle.ToInt64() | WS_EX_TRANSPARENT | WS_EX_LAYERED));
    }

    // restores normal window interaction by removing transparent style
    public static void SetWindowInteractable(IntPtr hwnd)
    {
        var extendedStyle = GetWindowLongPtr(hwnd, GWL_EXSTYLE);
        SetWindowLongPtr(hwnd, GWL_EXSTYLE, (IntPtr)(extendedStyle.ToInt64() & ~WS_EX_TRANSPARENT));
    }

    // applies toolwindow style, making it invisible to OBS and similar capture software
    public static void SetWindowToolWindow(IntPtr hwnd)
    {
        var extendedStyle = GetWindowLongPtr(hwnd, GWL_EXSTYLE);
        SetWindowLongPtr(hwnd, GWL_EXSTYLE, (IntPtr)(extendedStyle.ToInt64() | WS_EX_TOOLWINDOW));
    }

    // forces window to stay above all others using z-order manipulation
    public static void SetWindowTopMost(IntPtr hwnd)
    {
        SetWindowPos(hwnd, HWND_TOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_SHOWWINDOW);
    }
}
