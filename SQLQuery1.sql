select i.Id as 'id',i.title as 'title',d.name as 'department',u.UserName as ' name',i.likeNumber as 'like',i.viewNumber as 'view',i.yearOfPublish as'year',i.downloadNumber as 'download' from Items i inner join Courses c on i.CourseId=c.Id inner join Departments d on c.departmentId=d.Id 
 inner join AspNetUsers u on u.departmentId=d.Id 
  where u.Id= 'ebfcf9be-0d13-40c2-8c69-4943c22581d7'


 