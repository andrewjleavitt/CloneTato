using CloneTato.Assets;
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
}

public class GameStateManager
{
    public GameState State { get; } = new();
    public GameScreen CurrentScreen { get; private set; } = GameScreen.MainMenu;
    public GameScreen? PendingScreen { get; set; }
    public bool QuitRequested { get; set; }

    private readonly MainMenuScreen _mainMenu = new();
    private readonly CharacterSelectScreen _charSelect = new();
    private readonly PlayingScreen _playing = new();
    private readonly ShopScreen _shop = new();
    private readonly LevelUpScreen _levelUp = new();
    private readonly GameOverScreen _gameOver = new();
    private readonly VictoryScreen _victory = new();

    public void Init(AssetManager assets)
    {
        State.Assets = assets;
    }

    public void Update(float dt)
    {
        if (PendingScreen.HasValue)
        {
            CurrentScreen = PendingScreen.Value;
            PendingScreen = null;
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
        }
    }

    public void Draw()
    {
        switch (CurrentScreen)
        {
            case GameScreen.MainMenu: _mainMenu.Draw(State, this); break;
            case GameScreen.CharacterSelect: _charSelect.Draw(State, this); break;
            case GameScreen.Playing: _playing.Draw(State); break;
            case GameScreen.Shop: _shop.Draw(State, this); break;
            case GameScreen.LevelUp: _levelUp.Draw(State, this); break;
            case GameScreen.GameOver: _gameOver.Draw(State, this); break;
            case GameScreen.Victory: _victory.Draw(State, this); break;
        }
    }

    public void TransitionTo(GameScreen screen)
    {
        PendingScreen = screen;
    }
}
