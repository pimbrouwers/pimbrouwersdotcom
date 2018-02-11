const
  Ditto = require('ditt0'),
  DittoMarkdown = require('ditt0-markdown'),
  DittoHbs = require('ditt0-hbs'),
  DittoHtmlMinify = require('./minifier');



Ditto()
  .metadata({
    googleSiteVerification: 'iG1XdnX4f9LBgkELpy_fTAfKRLnLcLCWj6ZpC_NlbYI'
  })
  .use(new DittoMarkdown())
  .use(new DittoHbs({
    partials: './templates/partials',
    templates: './templates'
  }))
  .use(new DittoHtmlMinify({
    collapseWhitespace: true,
    minifyCSS: true,
    minifyJS: true
  }))
  .build();