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

