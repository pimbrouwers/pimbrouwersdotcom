var minimatch = require('minimatch');
/*
 * ditto HTML Minification Middleware
 */
module.exports = dittoSets;

function dittoSets(opt) {
  opt = opt || {};

  return function (files, ditto, done) {    
    let keys = Object.keys(opt);
    let setMatchers = dittoSetMatcher(keys, opt);

    //hydrate ditto global metadata with empty arrays to fill
    keys.forEach(function (key) {
      ditto._metadata[key] = [];
    });

    //iterate files and get matching keys (compared to original filename)
    files.forEach(function (file) {
      setMatchers(file.path.rel).forEach(function (key) {
        ditto._metadata[key].push(file);
      });
    });

    //sort sets
    keys.forEach(function (key) {
      let set = ditto._metadata[key];
      let setOpt = opt[key];

      //skip empty sets, and those without sort params
      if (set.length > 0 && setOpt.sort) {
        //was a sorting function provided?      
        if (typeof setOpt.sort == 'function') {
          set.sort(setOpt.sort);
        }
        //run vanilla sort based on 'sort' provided
        else {
          set.sort(function (a, b) {
            a = a[setOpt.sort];
            b = b[setOpt.sort];
            if (!a && !b) return 0;
            if (!a) return -1;
            if (!b) return 1;
            if (b > a) return -1;
            if (a > b) return 1;
            return 0;
          });
        }

        //if not 'asc' then reverse the set
        if (setOpt.asc === false) {
          set.reverse();
        }
      }
    });

    done(null, files);
  };
};

function dittoSetMatcher(keys, sets){
  let setMatchers = {};

  keys.forEach(function(key)
  {
    let set = sets[key];

    //skip set if no glob was specified
    if(!set.glob) {
      return;
    }

    //define isMatch
    setMatchers[key] = {
      isMatch: function(filepath){
        return minimatch(filepath, set.glob);
      }
    }
  });

  //expose as single function to check filepath against all matchers, returning array of keys
  return function(filePath){
    let keyMatches = [];

    Object.keys(setMatchers).forEach(function(key){
      let setMatcher = setMatchers[key];

      if(setMatcher.isMatch(filePath)){
        keyMatches.push(key);
      }
    });

    return keyMatches;
  };  
};