create table User
(
    Id bigint auto_increment primary key,
    UserName varchar(50) not null,
    Name varchar(50) null,
    LastName varchar(50) null,
    Password varchar(50) not null
);

create index UserName
    on User (UserName);