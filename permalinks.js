var path = require('path');

/*
 * Ditto Permalink Middleware
 */
module.exports = permalinks;

function permalinks() {

  return function (files, ditto, done) {    

    files.forEach(function (file) {      
      if (file.path != 'undefined' 
        && file.path != null 
        && file.path.name != 'index') {
        file.setDir(path.join(file.path.dir, file.path.name));
        file.setName('index');

        if(file.path.ext != '.html'){
          file.setExt('.html');
        }
      }
    });
    
    done(null, files);
  };
};
