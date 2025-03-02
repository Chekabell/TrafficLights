using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace traffic_lights;

public class TrafficLightViewModel : INotifyPropertyChanged
{
    private PeriodicTimer _timer;
    private StateTrafficLight _state;
    private bool _visibleRed = false;
    private bool _visibleYellow = false;
    private bool _visibleGreen = false;
    private bool _visibleWalkersRed = false;
    private bool _visibleWalkersGreen = false;

    private Color _buttonColor = Colors.Black;
    private bool _ButtonClicked = false;
    public Color ButtonColor{
        get => _buttonColor;
        set{
            _buttonColor = value;
            OnPropertyChanged();
        }
    }
    public StateTrafficLight State{
        get => _state;
        set{
            _state = value;
            OnPropertyChanged();
        }
    }
    public event PropertyChangedEventHandler? PropertyChanged;

    public ICommand WaitButtonCommand {get;}
    public bool VisibleRed{
        get => _visibleRed;
        set {
            _visibleRed = value;
            OnPropertyChanged();
        }
    }
    public bool VisibleYellow{
        get => _visibleYellow;
        set {
            _visibleYellow = value;
            OnPropertyChanged();
        }
    }
    public bool VisibleGreen{
        get => _visibleGreen;
        set {
            _visibleGreen = value;
            OnPropertyChanged();
        }
    }
    public bool VisibleWalkersRed{
        get => _visibleWalkersRed;
        set {
            _visibleWalkersRed = value;
            OnPropertyChanged();
        }
    }
    public bool VisibleWalkersGreen{
        get => _visibleWalkersGreen;
        set {
            _visibleWalkersGreen = value;
            OnPropertyChanged();
        }
    }

    public void OnPropertyChanged([CallerMemberName] string? prop = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        ((Command)WaitButtonCommand).ChangeCanExecute();
    }

    public enum StateTrafficLight
    {
        LightOff,
        LightRed,
        BlinkingGreenOnSecondSection,
        LightRedYellow,
        LightGreen,
        BlinkingGreen,
        LightYellow
    }
    
    public TrafficLightViewModel()
    {
        WaitButtonCommand = new Command(()=>{
            _ButtonClicked = true;
        }, () => State == StateTrafficLight.LightGreen);
        _timer = new PeriodicTimer(TimeSpan.FromSeconds(1));
        State = StateTrafficLight.LightOff;
        MainLoop();
    }

    private async void MainLoop()
    {
        await OffLight(3);
        State = StateTrafficLight.LightRed;
        await OnRed(8);
        while (true)
        {
            switch (State)
            {
                case StateTrafficLight.LightRed:
                    State = StateTrafficLight.BlinkingGreenOnSecondSection;
                    await BlinkingGreenOnSecondSection(1);
                    break;
                case StateTrafficLight.BlinkingGreenOnSecondSection:
                    State = StateTrafficLight.LightRedYellow;
                    await OnRedYellow(2);
                    break;
                case StateTrafficLight.LightRedYellow:
                    State = StateTrafficLight.LightGreen;
                    await OnGreen(8);
                    break;
                case StateTrafficLight.LightGreen:
                    State = StateTrafficLight.BlinkingGreen;
                    await OnBlinkingGreen(2);
                    ButtonColor = Colors.Black;
                    break;
                case StateTrafficLight.BlinkingGreen:
                    State = StateTrafficLight.LightYellow;
                    await OnYellow(1);
                    break;
                case StateTrafficLight.LightYellow:
                    State = StateTrafficLight.LightRed;
                    await OnRed(8);
                    break;
            }
        }
    }
    private async Task OffLight(double seconds)
    {
        while (seconds > 0 && await _timer.WaitForNextTickAsync())
        {
            seconds--;
        }
    }
    private async Task OnRed(double seconds)
    {
        VisibleRed = true;
        VisibleYellow = false;
        VisibleWalkersRed = false;
        VisibleWalkersGreen = true;
        while (seconds > 0 && await _timer.WaitForNextTickAsync())
        {
            seconds--;
        }
    }
    private async Task BlinkingGreenOnSecondSection(double seconds)
    {
        _timer.Period = TimeSpan.FromSeconds(0.5);
        while (seconds > 0 && await _timer.WaitForNextTickAsync())
        {
            VisibleWalkersGreen = false;
            await _timer.WaitForNextTickAsync();
            VisibleWalkersGreen = true;
            seconds -= 0.5;
        }
        _timer.Period = TimeSpan.FromSeconds(1);
        VisibleWalkersRed = true;
        VisibleWalkersGreen = false;
    }
    private async Task OnRedYellow(double seconds)
    {
        VisibleYellow = true;
        while (seconds > 0 && await _timer.WaitForNextTickAsync())
        {
            seconds--;
        }
    }
    private async Task OnGreen(double seconds)
    {
        VisibleRed = false;
        VisibleYellow = false;
        VisibleGreen = true;
        VisibleWalkersRed = true;
        VisibleWalkersGreen = false;
        while (seconds > 0 && await _timer.WaitForNextTickAsync())
        {
            if(_ButtonClicked){
                ButtonColor = Colors.DarkOrange;
                seconds = 2;
                _ButtonClicked = false;
            }
            seconds--;
        }
    }
    private async Task OnBlinkingGreen(double seconds)
    {
        _timer.Period = TimeSpan.FromSeconds(0.5);
        while (seconds > 0 && await _timer.WaitForNextTickAsync())
        {
            VisibleGreen = false;
            await _timer.WaitForNextTickAsync();
            VisibleGreen = true;
            seconds -= 0.5;
        }
        _timer.Period = TimeSpan.FromSeconds(1);
    }
    private async Task OnYellow(double seconds)
    {
        VisibleYellow = true;
        VisibleGreen = false;
        while (seconds > 0 && await _timer.WaitForNextTickAsync())
        {
            seconds--;
        }
    }

}

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
    }
}