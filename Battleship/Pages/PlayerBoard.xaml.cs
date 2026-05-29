using Battleship.BusinessLogic;
using Battleship.DataAccess;

namespace Battleship.Pages;

public partial class PlayerBoard
{
    private User _player1;
    private User _player2;
    private List<ShipItemModel> _player1ShipItemModels;
    private List<ShipItemModel> _player2ShipItemModels;
    private GameRecordCsvManager _dataManager;
    private List<string> letters = new List<string>() { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J" };
    private bool canPlayerHitCell = true;
    public static bool didIMakeFirstMove;
    public const string SHIP_COLOUR = "#73B8F6";
    private bool hasFirstCellBeenClicked;
    private ImageButton firstCellClicked;
    public static int turns;

    public PlayerBoard(User player1, User player2, List<ShipItemModel> player1ShipItemModels, List<ShipItemModel> player2ShipItemModels)
    {
        InitializeComponent();
        InitializeUIBoard();
        _player1 = player1;
        _player2 = player2;
        _player1ShipItemModels = player1ShipItemModels;
        _player2ShipItemModels = player2ShipItemModels;
        _dataManager = new GameRecordCsvManager(FileSystem.Current.AppDataDirectory);
        ShipsListView.ItemsSource = player1ShipItemModels;
        UpdateUIBoard();

        if (EnemyBoard.isGameInSetupPhase)
        {
            HeaderLabel.Text = "Pick a ship and click the grid to set its start and end position";
            HeaderFrame.BackgroundColor = Color.FromArgb("0C6CBA");
        }
        else
        {
            turns++;
            HeaderLabel.Text = "PLAYER 2 TURN: Click on any cell to fire";
            HeaderFrame.BackgroundColor = Color.FromArgb("FE6234");
            InitializeUITopLeftBoard();
        }
    }

    private void InitializeUIBoard()
    {
        int cellSize = 42;

        for (int row = 0; row < Board.GRID_SIZE + 1; row++)
        {
            BoardGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(cellSize) });
            if (row > 0)
            {
                Label numberLabel = new Label { FontSize = 15, TextColor = Color.FromArgb("2B2C2B"), Text = row.ToString(), VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Center };
                numberLabel.FontAttributes = FontAttributes.Bold;
                BoardGrid.Add(numberLabel, 0, row);
            }

            for (int col = 0; col < Board.GRID_SIZE + 1; col++)
            {
                BoardGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(cellSize) });
                if (col > 0 && row > 0)
                {
                    ImageButton cell = new ImageButton { Source = "empty.png", HeightRequest = cellSize + 2, WidthRequest = cellSize + 2, BackgroundColor = Color.FromRgba(1, 1, 1, 0.6) };
                    BoardGrid.Add(cell, col, row);
                    cell.Clicked += OnCellClicked;
                }
                if (row == 0 && col > 0)
                {
                    Label letterLabel = new Label { FontSize = 15, TextColor = Color.FromArgb("2B2C2B"), Text = letters[col - 1], VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Center };
                    letterLabel.FontAttributes = FontAttributes.Bold;
                    BoardGrid.Add(letterLabel, col, row);
                }
            }
        }
        BoardGrid.WidthRequest = cellSize * (Board.GRID_SIZE + 1);
        BoardGrid.HeightRequest = cellSize * (Board.GRID_SIZE + 1);
    }

    private void InitializeUITopLeftBoard()
    {
        int cellSize = 25;

        for (int row = 0; row < Board.GRID_SIZE; row++)
        {
            TopLeftBoard.RowDefinitions.Add(new RowDefinition { Height = new GridLength(cellSize) });

            for (int col = 0; col < Board.GRID_SIZE; col++)
            {
                string state = _player2.Board[row, col].State;
                TopLeftBoard.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(cellSize) });
                Image uiCell = new Image { Source = state + ".png", Aspect = Aspect.AspectFit, HeightRequest = cellSize, WidthRequest = cellSize, BackgroundColor = Color.FromRgba(1, 1, 1, 0.6) };
                TopLeftBoard.Add(uiCell, col, row);

                if (state == "ship")
                {
                    uiCell.BackgroundColor = Color.FromArgb(EnemyBoard.SHIP_COLOUR);
                }
            }
        }
        TopLeftBoard.WidthRequest = cellSize * Board.GRID_SIZE;
        TopLeftBoard.HeightRequest = cellSize * Board.GRID_SIZE;
    }

    private void UpdateUIBoard()
    {
        for (int i = 0; i < BoardGrid.Children.Count; i++)
        {
            var child = BoardGrid.Children[i];

            if (child is ImageButton)
            {
                ImageButton cell = child as ImageButton;
                string updatedCellState = _player1.Board[cell.GetRow(), cell.GetCol()].State;

                if (updatedCellState != cell.GetState())
                {
                    if (!(cell.GetState() == "first_cell_clicked" && updatedCellState != "ship"))
                    {
                        cell.SetState(updatedCellState);
                    }
                }

                if (cell.GetState() == "ship" && EnemyBoard.isGameInSetupPhase)
                {
                    cell.BackgroundColor = Color.FromArgb(SHIP_COLOUR);
                }
            }
        }
    }

    private void OnCellClicked(object sender, EventArgs e)
    {
        ImageButton cellClicked = sender as ImageButton;

        if (EnemyBoard.isGameInSetupPhase)
        {
            if (ShipsListView.SelectedItem != null)
            {
                int shipLength = ((ShipItemModel)ShipsListView.SelectedItem).Length;

                if (!hasFirstCellBeenClicked)
                {
                    firstCellClicked = cellClicked;

                    if (_player1.IsCellPlacementValid(firstCellClicked))
                    {
                        HandleFirstCellClicked(shipLength);
                    }
                    else
                    {
                        DisplayAlert("Invalid placement", "Ships can not be adjacent to one another or on another", "OK");
                        ShipsListView.SelectedItem = null;
                    }
                }
                else
                {
                    hasFirstCellBeenClicked = false;
                    ImageButton secondCellClicked = cellClicked;

                    if (_player1.IsCellPlacementValid(secondCellClicked))
                    {
                        ValidateAndDeployLongShip(secondCellClicked, shipLength);
                    }
                    else
                    {
                        DisplayAlert("Invalid placement", "Ships can not be adjacent to one another or on another", "OK");
                        firstCellClicked.SetState("empty");
                        UpdateUIBoard();
                    }
                    ShipsListView.SelectedItem = null;
                }
            }
        }
        else
        {
            if (canPlayerHitCell)
            {
                try
                {
                    string hitCellStatus = _player1.HitCell(cellClicked);
                    UpdateUIBoard();

                    if (hitCellStatus == "sunk")
                    {
                        foreach (ShipItemModel ship in _player1ShipItemModels)
                        {
                            if (ship.Length == _player1.Board[cellClicked.GetRow(), cellClicked.GetCol()].Ship.Length)
                            {
                                RemoveShipInListView(ship);

                                if (_player1.DidILose())
                                {
                                    GameOver();
                                }
                                return;
                            }
                        }
                    }
                    else if (hitCellStatus == "miss")
                    {
                        if (!didIMakeFirstMove)
                        {
                            _player2ShipItemModels = new List<ShipItemModel>
                            {
                                new ShipItemModel("length4_2.png", 4),
                                new ShipItemModel("length3_2.png", 3),
                                new ShipItemModel("length3_2.png", 3),
                                new ShipItemModel("length2_2.png", 2),
                                new ShipItemModel("length2_2.png", 2),
                                new ShipItemModel("length2_2.png", 2),
                                new ShipItemModel("length1_2.png", 1),
                                new ShipItemModel("length1_2.png", 1),
                                new ShipItemModel("length1_2.png", 1),
                                new ShipItemModel("length1_2.png", 1)
                            };
                        }
                        canPlayerHitCell = false;
                        didIMakeFirstMove = true;
                        NavigateToEnemyBoard();
                    }
                }
                catch (Exception ex)
                {
                    DisplayAlert("Invalid cell", ex.Message, "OK");
                }
            }
        }
    }

    private void HandleFirstCellClicked(int shipLength)
    {
        if (shipLength == 1)
        {
            List<(int, int)> coordinateOfShip = new List<(int, int)> { (firstCellClicked.GetRow(), firstCellClicked.GetCol()) };
            _player1.DeployShip(coordinateOfShip);
            UpdateUIBoard();
            RemoveShipInListView((ShipItemModel)ShipsListView.SelectedItem);
            ShipsListView.SelectedItem = null;
            firstCellClicked = null;
        }
        else
        {
            firstCellClicked.SetState("first_cell_clicked");
            hasFirstCellBeenClicked = true;
            UpdateUIBoard();
        }
    }

    private void ValidateAndDeployLongShip(ImageButton secondCellClicked, int shipLength)
    {
        bool didShipDeploy = _player1.GetCoordinatesOfShipIfPlacementIsValid(firstCellClicked, secondCellClicked, shipLength);

        if (didShipDeploy)
        {
            UpdateUIBoard();
            RemoveShipInListView((ShipItemModel)ShipsListView.SelectedItem);
        }
        else
        {
            DisplayAlert("Invalid placement", $"The two cells must form a straight line of {shipLength.ToString()} cells.", "OK");
            firstCellClicked.SetState("empty");
            UpdateUIBoard();
        }
    }

    private void OnShipSelected(object sender, EventArgs e)
    {
        if (hasFirstCellBeenClicked)
        {
            firstCellClicked.SetState("empty");
            firstCellClicked = null;
            hasFirstCellBeenClicked = false;
        }
    }

    private void RemoveShipInListView(ShipItemModel ship)
    {
        _player1ShipItemModels.Remove(ship);
        ShipsListView.ItemsSource = null;
        ShipsListView.ItemsSource = _player1ShipItemModels;

        if (_player1ShipItemModels.Count == 0 && EnemyBoard.isGameInSetupPhase)
        {
            NavigateToEnemyBoard();
        }
    }

    private async void NavigateToEnemyBoard()
    {
        await Task.Delay(TimeSpan.FromSeconds(0.3f));
        await Navigation.PushAsync(new EnemyBoard(_player1, _player2, _player1ShipItemModels, _player2ShipItemModels));
    }

    private async void GameOver()
    {
        _dataManager.SaveGameRecords(new GameRecord("P2", _player2.GetCountOfShipsLeft(), turns));
        await DisplayAlert("PLAYER 2 WINS", "", "Return to menu");
        await Navigation.PushAsync(new MainMenu());
    }
}