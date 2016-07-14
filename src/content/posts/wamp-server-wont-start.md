---
slug: wamp-server-wont-start
title: WAMP Server Won't Start
template: post.hbs
date: 2014-09-27
author: Pim Brouwers
tags: WAMP
---
WAMP Server is development platform for Windows (Windows Apache MySQL PHP). On occasion, when deploying a new copy of WAMP Server you may find that after enabling virtual hosts or modifying the configuration files, WAMP will no longer start. This is typically due to syntax errors in the aforementioned files.

- Go to start menu ->
- Type "cmd" ->
- Press enter ->
- Paste the following "C:\wamp\bin\apache\apache2.2.22\bin\httpd.exe" (note: your Apache version may be different)

This will indicate to you the exact line on which the error is occurring.