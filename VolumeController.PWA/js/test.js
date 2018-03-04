interact('.draggable').draggable({
    inertia: false,
    restrict: {
        restriction: ".groups",
        endOnly: true,
        elementRect: {
            top: 0,
            left: 0,
            bottom: 1,
            right: 1
        }
    },
    autoScroll: false,
    onmove: function (event) {
        var target = event.target,
            x = (parseFloat(target.getAttribute('data-x')) || 0) + event.dx,
            y = (parseFloat(target.getAttribute('data-y')) || 0) + event.dy;

        //console.log(target.getAttribute("can-move"));

        //if (target.getAttribute("can-move")) {
            target.style.webkitTransform = target.style.transform = 'translate(' + x + 'px, ' + y + 'px)';

            target.setAttribute('data-x', x);
            target.setAttribute('data-y', y);
        //}
    },
    onend: function (event) {
        var target = event.target;

        //var x = (parseFloat(target.getAttribute('data-sx')) || 0),
        //    y = (parseFloat(target.getAttribute('data-sy')) || 0);

        //target.setAttribute('data-x', x);
        //target.setAttribute('data-y', y);
    }
});

var sliders = $(".slider");
for (var i = 0; i < sliders.length; i++) {
    sliders[i].setAttribute("can-move", true);
}

$(".slider").on("mouseover", function (event) {
    [0].setAttribute("can-move", false);
});


interact('.dropzone').dropzone({
    accept: '.application',
    overlap: 'pointer',
    ondropactivate: function (event) {
        event.target.classList.add('drop-active');
    },
    ondragenter: function (event) {
        var draggableElement = event.relatedTarget,
            dropzoneElement = event.target;

        dropzoneElement.classList.add('drop-target');
        draggableElement.classList.add('can-drop');
    },
    ondragleave: function (event) {
        event.target.classList.remove('drop-target');
        event.relatedTarget.classList.remove('can-drop');
    },
    ondrop: function (event) {},
    ondropdeactivate: function (event) {
        event.target.classList.remove('drop-active');
        event.target.classList.remove('drop-target');
    }
});

interact(document).on('ready', function () {
    transformProp = 'transform' in document.body.style ?
        'transform' : 'webkitTransform' in document.body.style ?
        'webkitTransform' : 'mozTransform' in document.body.style ?
        'mozTransform' : 'oTransform' in document.body.style ?
        'oTransform' : 'msTransform' in document.body.style ?
        'msTransform' : null;
});