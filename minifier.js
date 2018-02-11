const minify = require('html-minifier').minify;

/*
 * Ditto HTML Minification Middleware
 */
module.exports = DittoHtmlMinify;

function DittoHtmlMinify(opt) {
  this.opt = opt || {};
};

/**
 * Ditto HTML Minification parsing middleware
 * @param {Array.<Object.<DittoFile>>} files 
 * @param {Object.<Ditto>} Ditto 
 * @param {Function} done 
 */
DittoHtmlMinify.prototype.run = function(files, Ditto, done) {
  let self = this;
  
  files.forEach(function(file) {
    if (file.content != 'undefined' && file.content != null) {
      let minified = minify(file.content, self.opt);
      
      if (minified != 'undefined' && minified != null) {
        file.content = minified;
      }
    }
  });

  done(null, files);
};