using Toybox.WatchUi as Ui;

class App04Delegate extends Ui.BehaviorDelegate {

    function initialize() {
        BehaviorDelegate.initialize();
    }

    function onMenu() {
        Ui.pushView(new Rez.Menus.MainMenu(), 
        new App04MenuDelegate(), Ui.SLIDE_UP);
        return true;
    }
}