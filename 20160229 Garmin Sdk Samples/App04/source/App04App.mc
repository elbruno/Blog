using Toybox.Application as App;
using Toybox.WatchUi as Ui;

class App04App extends App.AppBase {

    function initialize() {
        AppBase.initialize();
    }

    //! onStart() is called on application start up
    function onStart() {
    }

    //! onStop() is called when your application is exiting
    function onStop() {
    }

    //! Return the initial view of your application here
    function getInitialView() {
        return [ new App04View(), new App04Delegate() ];
    }

}
