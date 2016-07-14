---
slug: adding-style-to-cake-input-div-by-extending-formhelper
title: CakePHP - Adding Style to CakePHP Input Div by Extending FormHelper
template: post.hbs
date: 2014-11-07
author: Pim Brouwers
tags: CakePHP
---
Anyone arriving here looking for a way to add a class to input div wrappers application-wide (ex: if you're using a front-end framework which often requires a specific class name be added to input wrappers to enable auto-styles) there is a MUCH better solution. That being, a custom FormHelper.

- In the App/View/Helper directory create and save a file "MySuperCoolFormHelper.php"
- Place the following code in the file

```php
App::uses('FormHelper', 'View/Helper');

class MySuperCoolFormHelper extends FormHelper {

    protected function _divOptions($options) {
        if(isset($options['div'])
            $options['div'] .= ' class1 class2 class3'; //note the prefixing space
        else
            $options['div'] = 'class1 class2 class3';

        return parent::_divOptions($options);
    }
}
```
... and BLAMMO you're done!