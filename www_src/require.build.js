({
	include: ['requireLib'],
    mainConfigFile: "require.build.js",
    name: "startup",
    out: "../www_dist/scripts.js",
    paths: {
    	requireLib: 'require',
    },
    preserveLicenseComments: false,
})