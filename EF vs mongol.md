# Tárolt eljárás példa

## Új VAT beszúrása akkor, ha még nincs ilyen
```sql
create procedure addnewvat
	@percentage int
as
	begin
	
	begin tran
	set transaction isolation level repeatable read
	
	declare @count int
	
	select @count = count(*) from VAT where VAT.Percentage = @Percentage
	
	if @count = 0
		insert into VAT values (@Percentage)
	else
		print 'error'
	
	
	commit
end
```

# Trigger példa

## Trigger, hogy naprakész legyen a VAT-ban a ProductCount (hány productnak van ez a VAT-ja)
```sql
create trigger VatCount
on Product
for insert, update, delete
as

	update VAT
	set ProductCount = (select count(*) from Product where VAT.ID = Product.VATID)
	where ID in (select inserted.ID from inserted union select deleted.ID from deleted)
	
end
```