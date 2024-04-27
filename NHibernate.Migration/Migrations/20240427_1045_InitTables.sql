create table Books
(
    Id    bigint auto_increment
        primary key,
    Title varchar(50) not null
);

create table Chapters
(
    Id     bigint auto_increment
        primary key,
    Title  varchar(50) null,
    BookId bigint      null,
    constraint FK_C6DC97E
        foreign key (BookId) references Books (Id)
);

create index BookId
    on Chapters (BookId);

create table Genre
(
    Id    bigint auto_increment
        primary key,
    Title varchar(50) not null
);

create table BooksToGenres
(
    Book_id  bigint not null,
    Genre_id bigint not null,
    constraint FK_119B0C23
        foreign key (Genre_id) references Genre (Id),
    constraint FK_B0D5D88
        foreign key (Book_id) references Books (Id)
);

create index Book_id
    on BooksToGenres (Book_id);

create index Genre_id
    on BooksToGenres (Genre_id);

