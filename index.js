const
  ditto = require('ditt0'),
  markdown = require('ditt0-markdown'),
  hbs = require('ditt0-hbs'),
  minify = require('./minifier'),
  sets = require('./sets');

ditto()
  .clobber(true, '/*.!(css)')
  .metadata({
    googleSiteVerification: 'iG1XdnX4f9LBgkELpy_fTAfKRLnLcLCWj6ZpC_NlbYI',
    gravatar: 'https://www.gravatar.com/avatar/e7f73a5a9f3a2e698fa48353e3b10ebd.png'
  })
  .source('./src/pages/')  
  .use(markdown())
  .use(sets({
    posts: {
      glob: '[0-9]*/**/*.md',
      sort: 'date',
      asc: false
    }
  }))
  .use(hbs({
    partials: './src/templates/partials',
    templates: './src/templates'
  }))
  .use(minify({
    collapseWhitespace: true,
    minifyCSS: true,
    minifyJS: true
  }))
  .build();