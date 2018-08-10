const
  Ditto = require('ditt0'),
  DittoMarkdown = require('ditt0-markdown'),
  DittoHbs = require('ditt0-hbs'),
  DittoHtmlMinify = require('./minifier'),
  DittoSets = require('./sets');

Ditto()
  .clobber(true, '/*.!(css)')
  .metadata({
    googleSiteVerification: 'iG1XdnX4f9LBgkELpy_fTAfKRLnLcLCWj6ZpC_NlbYI',
    gravatar: 'https://www.gravatar.com/avatar/e7f73a5a9f3a2e698fa48353e3b10ebd.png'
  })
  .source('./src/pages')  
  .use(DittoMarkdown())
  .use(DittoSets({
    posts: {
      glob: 'posts/*.md',
      sort: 'date',
      asc: false
    }
  }))
  .use(DittoHbs({
    partials: './src/templates/partials',
    templates: './src/templates'
  }))
  .use(DittoHtmlMinify({
    collapseWhitespace: true,
    minifyCSS: true,
    minifyJS: true
  }))
  .build();