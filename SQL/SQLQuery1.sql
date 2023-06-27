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

SELECT id, name FROM Category ORDER BY name

UPDATE Category
                        SET
                            [Name] = @name
                        WHERE Id = 14;