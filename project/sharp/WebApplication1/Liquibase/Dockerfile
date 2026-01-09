FROM liquibase/liquibase:latest

# Install PostgreSQL JDBC driver
USER liquibase
RUN lpm add postgresql --global

# Copy changelog files
COPY --chown=liquibase:liquibase master.xml /liquibase/changelog/
COPY --chown=liquibase:liquibase change-sets/ /liquibase/changelog/change-sets/
WORKDIR /liquibase/changelog

ENTRYPOINT ["/liquibase/liquibase"]
CMD ["--changeLogFile=master.xml", "--url=jdbc:postgresql://db:5432/test_db", "--username=test_db", "--password=test_db", "--driver=org.postgresql.Driver", "update"]
