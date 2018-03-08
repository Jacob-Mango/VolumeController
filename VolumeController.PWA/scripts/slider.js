'use strict';

class Slider {
    constructor(id) {
        var self = this;
        this.value = 50;
        this.id = id;

        $("#" + id + ".slider-wrapper .slider-controller").mousemove(function (event) {

            console.log("moved");

            $("#" + self.id + ".slider-wrapper .slider-bar").css({
                "transform": "translateY(" + self.value + "%) "
            });
            $("#" + self.id + ".slider-wrapper .slider-controllerWrapper").css({
                "transform": "translateY(" + self.value + "%) "
            });
        });

        $("#" + id + ".slider-wrapper .slider-controller").click(function () {
            console.log("clicked");
            $("#" + id + ".slider-wrapper .slider-controller").mousemove();
        });
    }
};