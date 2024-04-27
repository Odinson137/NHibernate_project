SELECT COUNT(*) AS table_exists
FROM information_schema.tables
WHERE table_schema = 'NHibernateDb'
  AND table_name = 'Migrations';
