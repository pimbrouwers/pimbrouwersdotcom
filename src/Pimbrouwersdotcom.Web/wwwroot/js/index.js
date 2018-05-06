var ko = require('knockout'),
  SimpleMDE = require('simplemde');

ko.bindingHandlers.simpleMde = {
  init: function (element) {
    new SimpleMDE({ element: element });
  }
}

ko.applyBindings({});