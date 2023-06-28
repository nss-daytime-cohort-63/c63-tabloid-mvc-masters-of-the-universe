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

SELECT u.Id, u.FirstName, u.LastName, u.DisplayName, u.Email,
                               u.CreateDateTime, u.ImageLocation, u.UserTypeId,
                               ut.[Name] AS UserTypeName
                        FROM UserProfile u
                        LEFT JOIN UserType ut ON u.UserTypeId = ut.Id
                        WHERE u.Id = 1