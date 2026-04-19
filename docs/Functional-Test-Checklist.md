# BlindMatch PAS - Functional Test Checklist

## Test Environment
- OS: Windows
- Framework: ASP.NET Core MVC (.NET 10)
- Database: SQL Server / LocalDB
- Browser: Chrome / Edge
- Authentication: ASP.NET Core Identity
- Roles: Student, Supervisor, ModuleLeader, SystemAdmin

---

## FT-01 Register Student Account
**Precondition:** App is running  
**Steps:**
1. Open `/Identity/Account/Register`
2. Enter Full Name
3. Enter Email
4. Enter Password
5. Select Account Type = Student
6. Submit form

**Expected Result:**
- Account is created successfully
- User is signed in
- User is redirected to Student Dashboard

**Actual Result:** PASS / FAIL

---

## FT-02 Register Supervisor Account
**Precondition:** App is running  
**Steps:**
1. Open `/Identity/Account/Register`
2. Enter valid details
3. Select Account Type = Supervisor
4. Submit form

**Expected Result:**
- Account is created successfully
- User is signed in
- User is redirected to Supervisor Dashboard

**Actual Result:** PASS / FAIL

---

## FT-03 Student Can Submit Proposal
**Precondition:** Logged in as Student  
**Steps:**
1. Open `/Student/Dashboard`
2. Enter Title
3. Enter Technical Stack
4. Select Research Area
5. Enter Abstract
6. Submit

**Expected Result:**
- Proposal is saved
- Status is shown as `Pending`
- Proposal appears in "My Proposals"

**Actual Result:** PASS / FAIL

---

## FT-04 Student Can Upload Attachments
**Precondition:** Logged in as Student  
**Steps:**
1. Open `/Student/Dashboard`
2. Fill proposal form
3. Upload PDF and/or image
4. Submit

**Expected Result:**
- Proposal is saved
- Attachment links are displayed under the proposal
- Uploaded files open correctly

**Actual Result:** PASS / FAIL

---

## FT-05 Supervisor Can View Blind Proposals
**Precondition:** Logged in as Supervisor, at least one student proposal exists  
**Steps:**
1. Open `/Supervisor/Dashboard`

**Expected Result:**
- Supervisor can see Title, Abstract, Technical Stack, Research Area, and Status
- Supervisor cannot see student name/email before match confirmation

**Actual Result:** PASS / FAIL

---

## FT-06 Supervisor Can Express Interest
**Precondition:** Logged in as Supervisor, proposal exists  
**Steps:**
1. Open `/Supervisor/Dashboard`
2. Click `Express Interest`

**Expected Result:**
- Proposal status changes to `Under Review`
- Interest is recorded

**Actual Result:** PASS / FAIL

---

## FT-07 Supervisor Can Confirm Match
**Precondition:** Supervisor already expressed interest  
**Steps:**
1. Open `/Supervisor/Dashboard`
2. In "My Interested Proposals", click `Confirm Match`

**Expected Result:**
- Proposal status changes to `Matched`
- Match is confirmed
- Student identity is revealed to supervisor

**Actual Result:** PASS / FAIL

---

## FT-08 Student Sees Supervisor After Match
**Precondition:** Proposal is matched  
**Steps:**
1. Log in as Student
2. Open `/Student/Dashboard`

**Expected Result:**
- Proposal status shows `Matched`
- Supervisor name and email are visible

**Actual Result:** PASS / FAIL

---

## FT-09 Admin Can See Oversight Dashboard
**Precondition:** Logged in as admin  
**Steps:**
1. Open `/Admin/Dashboard`

**Expected Result:**
- Admin sees all proposals
- Admin sees status summary
- Matched proposals display both student and supervisor details
- Non-matched proposals keep blind information hidden where appropriate

**Actual Result:** PASS / FAIL

---

## FT-10 RBAC - Student Cannot Access Supervisor Dashboard
**Precondition:** Logged in as Student  
**Steps:**
1. Open `/Supervisor/Dashboard`

**Expected Result:**
- Access is denied

**Actual Result:** PASS / FAIL

---

## FT-11 RBAC - Supervisor Cannot Access Student Dashboard
**Precondition:** Logged in as Supervisor  
**Steps:**
1. Open `/Student/Dashboard`

**Expected Result:**
- Access is denied

**Actual Result:** PASS / FAIL

---

## FT-12 RBAC - Non-Admin Cannot Access Admin Dashboard
**Precondition:** Logged in as Student or Supervisor  
**Steps:**
1. Open `/Admin/Dashboard`

**Expected Result:**
- Access is denied

**Actual Result:** PASS / FAIL

---

## FT-13 Login Redirect by Role
**Precondition:** User account exists  
**Steps:**
1. Log in as Student
2. Log in as Supervisor
3. Log in as Admin

**Expected Result:**
- Student goes to Student Dashboard
- Supervisor goes to Supervisor Dashboard
- Admin goes to Admin Dashboard

**Actual Result:** PASS / FAIL

---

## FT-14 Invalid File Type Rejected
**Precondition:** Logged in as Student  
**Steps:**
1. Try uploading an invalid file type (e.g. `.exe`)
2. Submit proposal

**Expected Result:**
- Invalid file is not accepted
- Only allowed file types are processed

**Actual Result:** PASS / FAIL

---

## FT-15 Large File Handling
**Precondition:** Logged in as Student  
**Steps:**
1. Try uploading a file larger than allowed size
2. Submit proposal

**Expected Result:**
- File is rejected or skipped according to validation logic
- App does not crash

**Actual Result:** PASS / FAIL