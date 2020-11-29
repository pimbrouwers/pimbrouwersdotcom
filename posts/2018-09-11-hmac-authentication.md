[« Go home](/)

# HMAC Authentication: Better protection for your API

Building web software today is all about API's (application programming interface's). Whether your implementing your own microservice architecture or integrating with other platforms, there's just no way around it. And this is a good thing.

* * *

<small class="muted monospace">SEPTEMBER 11, 2018</small>

For the non-technical, an API is essentially a layer in front of your information that is designed to be computer-readable. If it could talk it would say, "this is the data I have to offer and this is what it looks like". Establishing a data contract of sorts. It is meant to be generic enough that the consumers (other software applications) don't need to know anything beyond the data format to consume it (this is not unlike HTTP itself, more on that another time). This means that the underlying API code could theoretically change at anytime without disrupting service, so long as the data contract is still met.

Still with me?

Most of the time the providers of this information don't wish to make it publicly accessible. Or they want to limit how much you can consume it. In order to do this, there needs to be a mechanism to identify you. To do this, providers will often use what's known as an Authentication HTTP Header, which is just a fancy way of saying "some additional information passed along with the URL when the request is made".

The bulk of API providers, not all, will typically resort to what's known as Basic Authentication, which is nothing more than a username and password combined with a colon and non-securely encoded into what's called a digest:

```csharp
// NOTE: this is pseudo-code
string usernameAndPassword = "username:password";
string digest = base64(usernameAndPassword); 
// result: dXNlcm5hbWU6cGFzc3dvcmQ=
```

This digest would then be used in an HTTP request to fetch information from the API:

```
GET /username/securedata
Host: somedomain.com
Authentication: Basic dXNlcm5hbWU6cGFzc3dvcmQ=
```

The main issue with this approach is that for a specific user, barring a password change, the hash is always the same. This means if anyone ever got a hold of this hash, they could make requests on your behalf. Because of this, it is never recommended to use Basic Authentication outside of SSL/TLS (https:// vs. http://).

This is where HMAC Authentication (hash based message authentication) comes into play. As denoted by its name, HMAC is still a hash, but a cryptographically secure one. To make it cryptographically secure usually I recommend using SHA-256 (secure hash algorithm) or stronger.

Opposed to sending the raw password on each request, a secure hash of the password and some other information is generated and sent in the HTTP Header. Often times this "other information" is the URI (universal resource indicator) and HTTP Verb (GET, POST etc.) of the request.

```csharp
// NOTE: this is pseudo-code   
string username = "username";
string password = "password";

var crytographer = hmac("SHA256", password);
string secureHash = crytographer.Hash("GET+/username/securedata");

// password is used as the key to generate 
// the hash from the message:
// GET+/username/securedata

string hmacHeader = base64(secureHash);

// base64 is used again to encode 
// the binary data into simple text
```

As before, this digest would then be used in an HTTP request to fetch information from the API (note the username passed in plain-text, this is used on the API side to lookup the user information):

```
GET /username/securedata
Host: somedomain.com
Authentication: hmac username:dXNlcm5hbWU6cGFzc3dvcmQ=
```

The powerful thing about using an HMAC digest in this way, is that it only grants the user access to the specific resource at the URI, /username/securedata in this case. What's more, additional information, most notably a timestamp, can be included which creates "decaying" digests. Meaning if someone obtains the token, they can only request a singular resource and for a limited time. Two big wins from a security point of view.

The process of creating, and deciphering HMAC digests is a little more time and CPU intensive. But given the merits, I believe the extra effort to be well worth it.