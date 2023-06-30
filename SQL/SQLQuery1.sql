SELECT * FROM Category;
SELECT * FROM Comment;
SELECT * FROM Post;
SELECT * FROM PostReaction;
SELECT * FROM PostTag;
SELECT * FROM Reaction;
SELECT * FROM Subscription;
SELECT * FROM Tag;
SELECT * FROM UserProfile;
SELECT * FROM UserType;

--Alter table UserProfile
--add IsActive bit not null default 1;

--alter table Comment
--drop constraint FK_Comment_Post;

--alter table Comment
--add constraint FK_Comment_Post
--foreign key ([PostId]) references [Post]([Id])
--on delete cascade;

UPDATE Post
SET IsApproved = 1
WHERE Id = 3;

SELECT u.Id, u.FirstName, u.LastName, u.DisplayName, u.Email,
                               u.CreateDateTime, u.ImageLocation, u.UserTypeId,
                               ut.[Name] AS UserTypeName
                        FROM UserProfile u
                        LEFT JOIN UserType ut ON u.UserTypeId = ut.Id
                        WHERE u.Id = 1

SELECT p.Id, p.Title, p.Content, 
                              p.ImageLocation AS HeaderImage,
                              p.CreateDateTime, p.PublishDateTime, p.IsApproved,
                              p.CategoryId, p.UserProfileId,
                              c.[Name] AS CategoryName,
                              u.FirstName, u.LastName, u.DisplayName, 
                              u.Email, u.CreateDateTime, u.ImageLocation AS AvatarImage,
                              u.UserTypeId, 
                              ut.[Name] AS UserTypeName
                         FROM Post p
                              LEFT JOIN Category c ON p.CategoryId = c.id
                              LEFT JOIN UserProfile u ON p.UserProfileId = u.id
                              LEFT JOIN UserType ut ON u.UserTypeId = ut.id
                        WHERE IsApproved = 1 AND PublishDateTime < SYSDATETIME()
                        ORDER BY PublishDateTime DESC

--INSERT INTO Reaction ( [Name], ImageLocation )
--    VALUES ( 'Like', 'https://cdn.cdnlogo.com/logos/f/41/facebook-reaction-like.svg' );

--INSERT INTO Reaction ( [Name], ImageLocation )
--    VALUES ( 'Love', 'https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcRmt2p6ZTgfjU4B0xc2CUu2XyJWFLqV8JtT3Q&usqp=CAU' );

--INSERT INTO Reaction ( [Name], ImageLocation )
--    VALUES ( 'Care', 'https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcTvd881WAmhuZg0Kbt_tLKY7goy3Yd6_CKGKQ&usqp=CAU' );

--INSERT INTO Reaction ( [Name], ImageLocation )
--    VALUES ( 'Wow', 'https://image.pngaaa.com/940/275940-middle.png' );

--INSERT INTO Reaction ( [Name], ImageLocation )
--    VALUES ( 'Sad', 'https://w7.pngwing.com/pngs/191/791/png-transparent-facebook-reaction-sad-hd-logo.png' );

--INSERT INTO Reaction ( [Name], ImageLocation )
--    VALUES ( 'Angry', 'https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcRLJwIyKPb8zosCthkcHGmVd6cAsZJk9GyqSBebYVtCiQIXQvPJRTi7f2RaiZS1w-AZpAs&usqp=CAU' );

SELECT r.Id, r.Name, r.ImageLocation, pr.Id, pr.PostId, pr.ReactionId, pr.UserProfileId
                        FROM PostReaction pr
                        JOIN Reaction r ON pr.ReactionId = r.Id
                        WHERE pr.PostId = 1
                        GROUP BY r.Id