// Sass configuration
var gulp = require('gulp');
var sass = require('gulp-sass');
var wait = require('gulp-wait');

gulp.task('sass', function () {
    gulp.src('./styles/main.scss')
        .pipe(wait(500))
        .pipe(sass())
        .pipe(gulp.dest(function (f) {
            return "styles";
        }));
});

gulp.task('default', ['sass'], function () {
    gulp.watch('./**/*.scss', ['sass']);
});