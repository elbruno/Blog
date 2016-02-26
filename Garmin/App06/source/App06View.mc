using Toybox.WatchUi as Ui;
using Toybox.Graphics as Gfx;
using Toybox.System as Sys;
using Toybox.Lang as Lang;

class App06View extends Ui.View {

    var _posInfo = null;

    function onLayout(dc) {
    }

    function onHide() {
    }

    function onShow() {
    }

    function onUpdate(dc) {
        var string;

        dc.setColor( Gfx.COLOR_TRANSPARENT, Gfx.COLOR_BLACK );
        dc.clear();
        dc.setColor( Gfx.COLOR_WHITE, Gfx.COLOR_TRANSPARENT );
        if( _posInfo != null ) {
            string = "El Bruno - Position";
            dc.drawText( (dc.getWidth() / 2), ((dc.getHeight() / 2) - 60), Gfx.FONT_MEDIUM , string, Gfx.TEXT_JUSTIFY_CENTER );
            string = "Lat = " + _posInfo.position.toDegrees()[0].toString();
            dc.drawText( (dc.getWidth() / 2), ((dc.getHeight() / 2) - 40), Gfx.FONT_SMALL, string, Gfx.TEXT_JUSTIFY_CENTER );
            string = "Long = " + _posInfo.position.toDegrees()[1].toString();
            dc.drawText( (dc.getWidth() / 2), ((dc.getHeight() / 2) - 20), Gfx.FONT_SMALL, string, Gfx.TEXT_JUSTIFY_CENTER );
            string = "speed = " + _posInfo.speed.toString();
            dc.drawText( (dc.getWidth() / 2), ((dc.getHeight() / 2) ), Gfx.FONT_SMALL, string, Gfx.TEXT_JUSTIFY_CENTER );
            string = "alt = " + _posInfo.altitude.toString();
            dc.drawText( (dc.getWidth() / 2), ((dc.getHeight() / 2) + 20), Gfx.FONT_SMALL, string, Gfx.TEXT_JUSTIFY_CENTER );
            string = "details = not defined";
            dc.drawText( (dc.getWidth() / 2), ((dc.getHeight() / 2) + 40), Gfx.FONT_SMALL, string, Gfx.TEXT_JUSTIFY_CENTER );
        }
        else {
            dc.drawText( (dc.getWidth() / 2), (dc.getHeight() / 2), Gfx.FONT_SMALL, "No position info", Gfx.TEXT_JUSTIFY_CENTER );
        }
    }

    function setPosition(info) {
        _posInfo = info;
        Ui.requestUpdate();
    }
}
