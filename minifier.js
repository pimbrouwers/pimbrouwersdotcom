const minify = require('html-minifier').minify;

/*
 * Ditto HTML Minification Middleware
 */
module.exports = dittoMinify;

function dittoMinify(opt) {
  opt = opt || {};

  return function (files, ditto, done) {    
    files.forEach(function (file) {      
      
      if (file.content != 'undefined' && file.content != null && file.content != '') {
        let minified = minify(file.content, opt);
        
        if (minified != 'undefined' && minified != null) {
          file.content = minified;
        }
      }
    });

    done(null, files);
  };
};
