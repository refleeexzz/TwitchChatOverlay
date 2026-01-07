# Twitch Chat Overlay

Uma aplicação WPF leve e robusta para exibir o chat da Twitch como uma sobreposição (overlay) transparente sobre jogos ou aplicações.

## ✨ Características Principais

### 🎯 Funcionalidades Implementadas

1. **Janela Transparente e Click-Through**
   - Overlay completamente transparente
   - Permite clicar através da janela (WS_EX_TRANSPARENT)
   - Background personalizável com opacidade ajustável

2. **Invisível para OBS e Softwares de Captura**
   - Usa `WS_EX_TOOLWINDOW` para não aparecer em capturas de tela
   - Perfeito para streaming sem exibir o overlay na transmissão
   - Configurável nas opções

3. **Dois Modos de Exibição**
   - **Modo Setup**: Com bordas, arrastável, redimensionável
   - **Modo Overlay**: Sem bordas, click-through, invisível no OBS

4. **Configurações Persistentes**
   - Salva automaticamente posição e tamanho da janela
   - Configurações de opacidade, zoom e comportamento
   - Arquivo de configuração em JSON (`%APPDATA%\TwitchChatOverlay\settings.json`)

5. **Controles de Opacidade**
   - Ajuste fino de 0-255 (0% a 100%)
   - Atalhos rápidos para aumentar/diminuir
   - Visualização em tempo real

6. **Interface Intuitiva**
   - Menu de contexto (botão direito)
   - Botões de controle no cabeçalho
   - Design moderno inspirado no Twitch

## 🚀 Como Usar

### Inicialização

1. Execute o aplicativo
2. Por padrão, abre em **Modo Setup** (com bordas visíveis)
3. Configure o canal da Twitch nas configurações

### Controles Principais

#### Botões do Cabeçalho

- **⚙ (Configurações)**: Abre o diálogo de configurações
- **○ (Toggle Borders)**: Alterna entre Modo Setup e Modo Overlay
- **✕ (Fechar)**: Fecha a aplicação

#### Menu de Contexto (Botão Direito)

- **Toggle Borders**: Alterna entre os modos
- **Settings**: Abre configurações
- **Opacity**: Submenu para ajustar opacidade
  - Increase Opacity (+15)
  - Decrease Opacity (-15)
  - Reset Opacity (50%)
- **Reset Window Position**: Restaura posição padrão
- **Exit**: Fecha a aplicação

### Modos de Operação

#### 🔧 Modo Setup
- **Uso**: Configuração e posicionamento
- **Características**:
  - Bordas visíveis
  - Janela arrastável (clique e arraste)
  - Redimensionável (cantos/bordas)
  - Ícone na barra de tarefas visível
  - Totalmente interativo

#### 🎮 Modo Overlay
- **Uso**: Durante jogos/streaming
- **Características**:
  - Sem bordas
  - Click-through (cliques passam através)
  - Invisível para OBS
  - Sem ícone na barra de tarefas (opcional)
  - Sempre no topo

## ⚙️ Configurações Detalhadas

### Twitch Settings
- **Channel Name**: Nome do canal da Twitch para conectar

### Appearance
- **Opacity Level**: Transparência do background (0-100%)
  - 0% = Completamente transparente
  - 100% = Completamente opaco
- **Zoom Level**: Nível de zoom (50%-200%)

### Behavior
- **Auto-hide borders on startup**: Inicia automaticamente em Modo Overlay
- **Hide from OBS / Screen Capture**: Torna a janela invisível para captura
- **Hide taskbar icon in overlay mode**: Oculta ícone da taskbar no Modo Overlay

## 🛠️ Arquitetura Técnica

### Estrutura do Projeto

```
TwitchChatOverlay/
├── MainWindow.xaml              # Interface principal
├── MainWindow.xaml.cs           # Lógica da janela principal
├── SettingsDialog.xaml          # Interface de configurações
├── SettingsDialog.xaml.cs       # Lógica de configurações
├── WindowHelper.cs              # Helpers Win32 API
├── WindowDisplayMode.cs         # Enum dos modos
├── AppSettings.cs               # Gerenciamento de configurações
└── TwitchChatOverlay.csproj     # Arquivo do projeto
```

### Tecnologias Utilizadas

- **.NET 10.0** (ou mais recente)
- **WPF** (Windows Presentation Foundation)
- **Win32 API** para manipulação avançada de janelas
- **JSON** para persistência de configurações

### Windows API Utilizadas

```csharp
// Click-through
WS_EX_TRANSPARENT = 0x20

// Suporte a transparência
WS_EX_LAYERED = 0x80000

// Invisível para OBS
WS_EX_TOOLWINDOW = 0x80
```

## 📝 Próximos Passos (Roadmap)

Para completar a integração com Twitch:

### 1. Integração TwitchLib
```bash
dotnet add package TwitchLib
```

### 2. Implementar Conexão IRC
```csharp
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;

// Adicionar no MainWindow.xaml.cs
private TwitchClient _twitchClient;

private void ConnectToTwitch()
{
    var credentials = new ConnectionCredentials("justinfan12345", "oauth:...");
    _twitchClient = new TwitchClient();
    _twitchClient.Initialize(credentials, _settings.TwitchChannel);
    
    _twitchClient.OnMessageReceived += OnMessageReceived;
    _twitchClient.Connect();
}

private void OnMessageReceived(object sender, OnMessageReceivedArgs e)
{
    AddChatMessage(e.ChatMessage.Username, e.ChatMessage.Message);
}
```

### 3. Recursos Adicionais Sugeridos
- ✅ Emotes (BTTV, FFZ, 7TV)
- ✅ Badges de usuários
- ✅ Cores personalizadas de username
- ✅ Sons de notificação
- ✅ Filtros de mensagens
- ✅ Suporte a recompensas de pontos de canal
- ✅ Hotkeys globais (ex: Ctrl+Alt+O para toggle borders)
- ✅ Auto-reconnect ao perder conexão

## 🎨 Personalização

### Alterar Cores do Tema

Edite `MainWindow.xaml`:

```xaml
<!-- Background do overlay -->
<Border Background="#80000000" ... />

<!-- Header -->
<Grid Background="#CC6441A5" ... />

<!-- Footer -->
<Grid Background="#CC1F1F23" ... />
```

### Alterar Cor dos Usernames

Em `MainWindow.xaml`, no DataTemplate:

```xaml
<Run Text="{Binding Username}" 
     FontWeight="Bold" 
     Foreground="#FF9146FF"/>  <!-- Altere esta cor -->
```

## 🐛 Solução de Problemas

### A janela não fica click-through
- Certifique-se de estar em **Modo Overlay**
- Verifique se `WS_EX_TRANSPARENT` está sendo aplicado

### A janela aparece no OBS
- Ative "Hide from OBS / Screen Capture" nas configurações
- Reinicie a aplicação após alterar a configuração

### A janela desaparece
- Pressione `Ctrl+Alt` e clique na área onde a janela estava
- Use "Reset Window Position" no menu de contexto

### Configurações não são salvas
- Verifique permissões da pasta `%APPDATA%\TwitchChatOverlay`
- Execute como administrador se necessário

## 📄 Licença

Este projeto é fornecido como está, para fins educacionais e de uso pessoal.

## 🙏 Créditos

Inspirado em:
- [Transparent-Twitch-Chat-Overlay](https://github.com/baffler/Transparent-Twitch-Chat-Overlay) por baffler
- Comunidade Twitch e desenvolvedores de overlays

## 💬 Suporte

Para problemas ou sugestões, abra uma issue no repositório do projeto.

---

**Desenvolvido com ❤️ para a comunidade de streamers**
