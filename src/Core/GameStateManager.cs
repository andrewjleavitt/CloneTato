using CloneTato.Assets;
using CloneTato.Data;
using CloneTato.Screens;

namespace CloneTato.Core;

public enum GameScreen
{
    MainMenu,
    CharacterSelect,
    Playing,
    Shop,
    LevelUp,
    GameOver,
    Victory,
    WeaponGallery,
    MetaUpgrades,
}

public enum OverlayScreen
{
    None,
    Pause,
    Settings,
}

public class GameStateManager
{
    public GameState State { get; } = new();
    public MetaProgression Meta { get; private set; } = null!;
    public GameScreen CurrentScreen { get; private set; } = GameScreen.MainMenu;
    public GameScreen? PendingScreen { get; set; }
    public bool QuitRequested { get; set; }
    public bool IsPaused { get; private set; }
    public OverlayScreen ActiveOverlay { get; private set; } = OverlayScreen.None;

    private readonly MainMenuScreen _mainMenu = new();
    private readonly CharacterSelectScreen _charSelect = new();
    private readonly PlayingScreen _playing = new();
    private readonly ShopScreen _shop = new();
    private readonly LevelUpScreen _levelUp = new();
    private readonly GameOverScreen _gameOver = new();
    private readonly VictoryScreen _victory = new();
    private readonly WeaponGalleryScreen _weaponGallery = new();
    private readonly MetaUpgradeScreen _metaUpgrades = new();
    private readonly PauseScreen _pause = new();
    private readonly SettingsScreen _settings = new();

    private OverlayScreen _settingsReturnOverlay;

    public void Init(AssetManager assets)
    {
        State.Assets = assets;
        Meta = MetaProgression.Load();
    }

    public void Pause()
    {
        IsPaused = true;
        ActiveOverlay = OverlayScreen.Pause;
        _pause.Reset();
    }

    public void Unpause()
    {
        IsPaused = false;
        ActiveOverlay = OverlayScreen.None;
    }

    public void OpenSettings()
    {
        _settingsReturnOverlay = ActiveOverlay;
        ActiveOverlay = OverlayScreen.Settings;
        _settings.Reset();
    }

    public void CloseSettings()
    {
        ActiveOverlay = _settingsReturnOverlay;
        // If we came back to None and were paused, that means settings was opened
        // from a non-pause context (main menu) — ensure pause state is clean
        if (ActiveOverlay == OverlayScreen.None)
            IsPaused = false;
    }

    public void Update(float dt)
    {
        if (PendingScreen.HasValue)
        {
            CurrentScreen = PendingScreen.Value;
            PendingScreen = null;
        }

        // Handle pause toggle during gameplay
        if (CurrentScreen == GameScreen.Playing && ActiveOverlay == OverlayScreen.None
            && InputHelper.IsPausePressed())
        {
            Pause();
            return;
        }

        // Handle overlays
        if (ActiveOverlay != OverlayScreen.None)
        {
            switch (ActiveOverlay)
            {
                case OverlayScreen.Pause:
                    _pause.Update(dt, State, this);
                    break;
                case OverlayScreen.Settings:
                    _settings.Update(dt, State, this);
                    break;
            }
            return;
        }

        switch (CurrentScreen)
        {
            case GameScreen.MainMenu: _mainMenu.Update(dt, State, this); break;
            case GameScreen.CharacterSelect: _charSelect.Update(dt, State, this); break;
            case GameScreen.Playing: _playing.Update(dt, State, this); break;
            case GameScreen.Shop: _shop.Update(dt, State, this); break;
            case GameScreen.LevelUp: _levelUp.Update(dt, State, this); break;
            case GameScreen.GameOver: _gameOver.Update(dt, State, this); break;
            case GameScreen.Victory: _victory.Update(dt, State, this); break;
            case GameScreen.WeaponGallery: _weaponGallery.Update(dt, State, this); break;
            case GameScreen.MetaUpgrades: _metaUpgrades.Update(dt, State, this); break;
        }
    }

    public void Draw()
    {
        // Always draw the current screen
        switch (CurrentScreen)
        {
            case GameScreen.MainMenu: _mainMenu.Draw(State, this); break;
            case GameScreen.CharacterSelect: _charSelect.Draw(State, this); break;
            case GameScreen.Playing: _playing.Draw(State); break;
            case GameScreen.Shop: _shop.Draw(State, this); break;
            case GameScreen.LevelUp: _levelUp.Draw(State, this); break;
            case GameScreen.GameOver: _gameOver.Draw(State, this); break;
            case GameScreen.Victory: _victory.Draw(State, this); break;
            case GameScreen.WeaponGallery: _weaponGallery.Draw(State, this); break;
            case GameScreen.MetaUpgrades: _metaUpgrades.Draw(State, this); break;
        }

        // Draw overlay on top
        switch (ActiveOverlay)
        {
            case OverlayScreen.Pause:
                _pause.Draw(State, this);
                break;
            case OverlayScreen.Settings:
                _settings.Draw(State, this);
                break;
        }
    }

    public void TransitionTo(GameScreen screen)
    {
        PendingScreen = screen;
    }
}
