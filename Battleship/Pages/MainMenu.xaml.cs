using Battleship.BusinessLogic;
using Battleship.DataAccess;

namespace Battleship.Pages;

public partial class MainMenu : ContentPage
{
    private GameRecordCsvManager _dataManager;

    public MainMenu()
    {
        InitializeComponent();
        _dataManager = new GameRecordCsvManager(FileSystem.Current.AppDataDirectory);
    }

    private List<ShipItemModel> player1ShipItemModels = new List<ShipItemModel>
    {
        new ShipItemModel("length4.png", 4),
        new ShipItemModel("length3.png", 3),
        new ShipItemModel("length3.png", 3),
        new ShipItemModel("length2.png", 2),
        new ShipItemModel("length2.png", 2),
        new ShipItemModel("length2.png", 2),
        new ShipItemModel("length1.png", 1),
        new ShipItemModel("length1.png", 1),
        new ShipItemModel("length1.png", 1),
        new ShipItemModel("length1.png", 1)
    };

    private List<ShipItemModel> player2ShipItemModels = new List<ShipItemModel>
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

    private async void PlayButton_Click(object sender, EventArgs e)
    {
        EnemyBoard.isGameInSetupPhase = true;
        User player1 = new User();
        User player2 = new User();
        await Navigation.PushAsync(new PlayerBoard(player1, player2, player1ShipItemModels, player2ShipItemModels));
    }

    private void ViewMatchHistory(object sender, EventArgs e)
    {
        DisplayAlert("Match History", _dataManager.ReadGameRecords(), "OK");
    }
}




