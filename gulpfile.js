var fs = require("fs");
var path = require("path");
var gulp = require("gulp");
var child_process = require('child_process');
var through = require('through2');
var mime = require('mime');
var OSS = require('ali-oss').Wrapper;

var accessKeyId = process.env.accessKeyId;
var accessKeySecret = process.env.accessKeySecret;

var client = new OSS({
  accessKeyId: accessKeyId,
  accessKeySecret: accessKeySecret,
  bucket: 'plugins-siteserver-cn',
  region: 'oss-cn-zhangjiakou'
});
var ossPrefix = 'ss.form';

var upload = function (relatedPath, buff, contentType) {
  client.put(relatedPath, buff, {
    mime: contentType ? contentType : mime.getType(relatedPath)
  }).catch(function (err) {
    console.log('error: %j', err);
  });
}

gulp.task('publish', function () {
  var dirname = __dirname;

  return gulp.src(['./index.html', './logo.svg', './docs/**/*'])
    .pipe(through.obj(function (file, enc, cb) {
      if (file.contents) {
        var filePath = file.path;
        var relatedPath = file.path.substr(dirname.length);
        relatedPath = relatedPath.replace(/\\/g, '/').replace(/(^\/*)|(\/*$)/g, "");
        relatedPath = ossPrefix + '/' + relatedPath
        console.log(relatedPath);

        upload(relatedPath, file.contents);
        if (filePath.endsWith('index.html')) {
          console.log(relatedPath.substr(0, relatedPath.length - 10));
          console.log(relatedPath.substr(0, relatedPath.length - 11));
          upload(relatedPath.substr(0, relatedPath.length - 10), file.contents, 'text/html');
          upload(relatedPath.substr(0, relatedPath.length - 11), new Buffer('<html><head><meta charset="utf-8"><script>location.href="/' + relatedPath.substr(0, relatedPath.length - 10) + '";</script></head><body></body></html>'), 'text/html');
        }
      }

      cb(null, file);
    }));
});