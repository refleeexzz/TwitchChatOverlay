# Twitch Chat Overlay

A lightweight desktop app that displays Twitch chat as a transparent overlay on top of your games. Perfect for single-monitor setups when streaming or gaming.

## What it does

This overlay window sits on top of your game and shows your Twitch chat in real-time. The cool part? You can click right through it to interact with your game, and it won't show up in your OBS stream capture.

## Key Features

- **Click-through overlay** - your mouse clicks pass through the window to the game behind it
- **Invisible to stream software** - won't appear in OBS or similar capture programs
- **Two modes**: Setup mode (can move/resize) and Overlay mode (locked in place)
- **System tray controls** - right-click the tray icon to access all settings
- **Adjustable transparency** - make it as visible or subtle as you want
- **Always on taskbar** - easy to find and manage your overlay window
- **Saves your preferences** - remembers window position, size, and settings

## Quick Start

1. Launch the app - it opens in Setup mode with visible borders
2. Right-click the system tray icon (near your clock)
3. Select "Set Channel..." and enter your Twitch channel name
4. Position and resize the window where you want it
5. Right-click tray icon → "Toggle Borders" to lock it in Overlay mode
6. Start gaming - chat will display without blocking your clicks

## Using the Tray Icon

The system tray icon (bottom-right corner of Windows) gives you quick access to everything:

**Right-click menu options:**
- **Set Channel** - choose which Twitch channel to display
- **Toggle Borders** - switch between Setup and Overlay modes
- **Opacity** - adjust transparency (increase/decrease/reset)
- **Settings** - open full settings window
- **Reset Window Position** - move window back to default location
- **Exit** - close the app

**Left-click** - shows/focuses the main window

## Display Modes

### Setup Mode (borders visible)
Use this when you want to move or resize the window:
- Drag anywhere to move
- Resize from corners/edges
- Visible in taskbar
- Can interact with buttons

### Overlay Mode (borderless)
Use this during gaming/streaming:
- No borders or buttons
- Click-through enabled
- Hidden from OBS capture
- Locked position
- Still visible in taskbar

## Settings Window

Access via tray icon → Settings:

**Twitch Settings**
- Channel Name - which stream's chat to display

**Appearance**
- Opacity Level - how transparent the background is (0-100%)
- Zoom Level - scale the chat size (50-200%)

**Behavior**
- Auto-hide borders on startup - launches directly in Overlay mode
- Hide from OBS/Screen Capture - makes overlay invisible to streaming software
- Hide taskbar icon in overlay mode - removes from taskbar when in Overlay mode

## Tips

- The overlay remembers your last position and size
- Use keyboard shortcuts with the tray menu for faster adjustments
- Keep opacity around 50% for best visibility without blocking your view
- Settings are automatically saved when you close the app

## Technical Details

Built with .NET and WPF for Windows. Uses native Windows APIs to achieve click-through functionality and stream capture invisibility.

**System Requirements**
- Windows 10 or later
- .NET 10.0 runtime

## Roadmap

Future additions planned:
- TwitchLib integration for live chat connection
- Emote support (BTTV, FFZ, 7TV)
- Badge display for subscribers/mods
- Custom styling options
- Sound notifications
- Message filtering
- Global hotkeys
## Credits

Inspired by the transparent overlay projects in the streaming community, particularly the work on [Transparent-Twitch-Chat-Overlay](https://github.com/baffler/Transparent-Twitch-Chat-Overlay).

## License

Free to use for personal streaming and gaming purposes.

---

**Made for streamers who need their chat on a single monitor** 🎮💜

Inspirado em:
- [Transparent-Twitch-Chat-Overlay](https://github.com/baffler/Transparent-Twitch-Chat-Overlay) por baffler
- Comunidade Twitch e desenvolvedores de overlays

## 💬 Suporte

Para problemas ou sugestões, abra uma issue no repositório do projeto.

---

**Desenvolvido com ❤️ para a comunidade de streamers**
