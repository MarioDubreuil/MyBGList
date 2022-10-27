-- delete from Domains;
select * from Domains;
-- insert into Domains values (1, 'domain 1', now(), now());
-- insert into Domains values (2, 'domain 2', now(), now());
-- insert into Domains values (3, 'domain 3', now(), now());
-- alter table Domains auto_increment = 1;

select * from Mechanics;
-- insert into Mechanics values (1, 'mechanics 1', now(), now());
-- insert into Mechanics values (2, 'mechanics 2', now(), now());
-- insert into Mechanics values (3, 'mechanics 3', now(), now());

select * from BoardGames;
-- insert into BoardGames values (1, 'board game 1', 2017, 2, 4, 30, 12, 97, 4.2, 13, 7.4, 35, now(), now());
-- insert into BoardGames values (2, 'board game 2', 1978, 1, 2, 15, 6, 185, 2.3, 275, 4.1, 745, now(), now());

select * from BoardGames_Domains;
-- insert into BoardGames_Domains values (1, 2, now());
-- insert into BoardGames_Domains values (1, 3, now());
-- insert into BoardGames_Domains values (2, 2, now());

select * from BoardGames_Mechanics;
-- insert into BoardGames_Mechanics values (1, 1, now());
-- insert into BoardGames_Mechanics values (2, 1, now());
-- insert into BoardGames_Mechanics values (2, 2, now());

select *
from BoardGames bg
left outer join BoardGames_Domains bg_d on bg_d.BoardGameId = bg.Id
left outer join Domains d on d.Id = bg_d.DomainId;

select *
from BoardGames bg
left outer join BoardGames_Mechanics bg_m on bg_m.BoardGameId = bg.Id
left outer join Mechanics m on m.Id = bg_m.MechanicId;

select * from BoardGames where Id in (1, 2);
select * from BoardGames_Domains where BoardGameId in (1, 2);
select * from BoardGames_Mechanics where BoardGameId in (1, 2);
select * from Domains where Id in (1, 2, 3);
select * from Mechanics where Id in (1, 2, 3);

-- delete from BoardGames where Id in (1, 2);
-- delete from Domains where Id in (1, 2, 3);
-- delete from Mechanics where Id in (1, 2, 3);

select * from Domains;
select * from Mechanics;
select count(*) from BoardGames;
select * from BoardGames;

select Name, count(*), min(id), max(Id)
from BoardGames
group by Name
having count(*) > 1
order by 1;