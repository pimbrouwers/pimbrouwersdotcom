---
slug: cakephp-access-associated-model-in-beforesave
title: CakePHP - How to Access an Associated Model in beforeSave Function
template: post.hbs
date: 2014-09-27
author: Pim Brouwers
tags: CakePHP
---
In CakePHP, as with any other MVC framework, we often need access to the models directly associated to the one currently being acted upon. In Cake > 2.x this is a simple procedure. I'll explain using an example.

- We have a model Subscription which belongsTo a SubscriptionsPlan
- Model SubscriptionsPlan hasMany Suscriptions
To access the SubscriptionsPlan data in a beforeSave function in the Subscription model, you would do the following:
```php
public function beforeSave($options = array()){
    $options = array(
        'conditions' => array(
            'SubscriptionsPlan.subscriptions_plan_id' => $this->data[$this->alias]['subscriptions_plan_id']
        )
    );

    $plan = $this->SubscriptionsPlan->find('first', $options);

    //REST OF BEFORE SAVE CODE GOES HERE
    return true;
}
```