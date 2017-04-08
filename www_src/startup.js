require.config({});

var App = null;

require(['jquery', 'knockout', 'js/app', 'js/dom-bindings'], function($, ko, AppModel, DomBindings){
    App = new AppModel();
    App.Initialize();
    
    DomBindings.Register();    
});