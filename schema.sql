drop table if exists PostTag;
drop table if exists Tag;
drop table if exists Post;

create table Account (
  Id integer primary key autoincrement
  ,Username varchar(255)
  ,Password char(88)
  ,Salt char(55)
;  

create table Post (
	Id integer primary key autoincrement
	,Title nvarchar(100) not null
	,Tldr nvarchar(255) not null
	,Body nvarchar(4000) not null
	,Dt datetime not null
);

create table Tag (
  Id integer primary key autoincrement
  ,Label nvarchar(255) not null
);

create table PostTag (
  PostId integer
  ,TagId integer
  ,primary key (PostId, TagId)
  ,foreign key (PostId) references Post (Id)
  ,foreign key (TagId) references Tag (Id)
);

insert into Account (
  Username
  ,Password
  ,Salt
)
values (
  'me'
  ,'zmUYkM/AQ2y+l2aTzDG1GcIWaB9ZLpNXXur40qAwA+dEuT7zdlqLxbJ8L+xPbngLkjdinlkjgo3Nu/FYJ/WCAA=='
  ,'100000.g9Ubb9Pw13J2B8Ht4gC+ukQCA4tXPkB64mpqtpJ+JFFaiA=='
);

create index ix_account_username on Account(Username);
create index ix_post_dt on Post(Dt);
create index ix_tag_label on Tag(Label);