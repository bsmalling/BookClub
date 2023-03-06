-- Run once
INSERT INTO [AspNetRoles]
    ([Id], [Name], [NormalizedName], [ConcurrencyStamp])
VALUES
    (NEWID(), 'Guest', 'Guest', NEWID()),
    (NEWID(), 'User',  'User',  NEWID()),
    (NEWID(), 'Super', 'Super', NEWID()),
    (NEWID(), 'Admin', 'Admin', NEWID());

-- Run per user
INSERT INTO [AspNetUserClaims]
	([UserId], [ClaimType], [ClaimValue])
VALUES
	('64d4c7af-2a19-4bd1-ab22-393676f437b4', 'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname', 'Brad'),
	('64d4c7af-2a19-4bd1-ab22-393676f437b4', 'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/surname', 'Smalling'),
	('64d4c7af-2a19-4bd1-ab22-393676f437b4', 'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/userdata', 'Married with two adult sons.');

-- Run per user
INSERT INTO [AspNetUserRoles]
	([UserId], [RoleId])
VALUES
	('64d4c7af-2a19-4bd1-ab22-393676f437b4', 'E136DC1E-4467-426C-BEE2-8DE53B0FFB67');
