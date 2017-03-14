using Toybox.WatchUi as Ui;
using Toybox.Graphics as Gfx;
using Toybox.System as Sys;
using Toybox.Lang as Lang;

class App03View extends Ui.WatchFace {

	var hrData;

    function initialize() {
        WatchFace.initialize();
    	Sensor.setEnabledSensors( [Sensor.SENSOR_HEARTRATE] );
    	Sensor.enableSensorEvents( method( :onSensor ) );
    }
    
    function onSensor(sensorInfo) {
    	hrData = "Hr(" + sensorInfo.heartRate +")";
    	System.println( hrData );
    	Ui.requestUpdate();
    }

    function onLayout(dc) {
        setLayout(Rez.Layouts.WatchFace(dc));
    }

    function onUpdate(dc) {
        var clockTime = Sys.getClockTime();
        var infoString = Lang.format("$1$:$2$", [clockTime.hour, clockTime.min.format("%02d")]);
        infoString = infoString + " " + hrData;
        var view = View.findDrawableById("TimeLabel");
        view.setText(infoString);
        View.onUpdate(dc);
    }

    function onShow() {
    }

    function onHide() {
    }

    //! The user has just looked at their watch. Timers and animations may be started here.
    function onExitSleep() {
    }

    //! Terminate any active timers and prepare for slow updates.
    function onEnterSleep() {
    }

}
