#noinspection CucumberUndefinedStep
Feature: Adze
  Adze provides a decoupled layer to external advertising services.

  Rule: First passing service will respond to a display request

  Background:
    Given 4 services available
    And they are ordered  as "Round Robin"

  @RoundRobinAllPass
  Example: The first service responds
    Given that the all services work
    When I ask for an advertisement
    Then I get the service 0
    When I ask for an advertisement
    Then I get the service 1
    When I ask for an advertisement
    Then I get the service 2
    When I ask for an advertisement
    Then I get the service 3
    When I ask for an advertisement
    Then I get the service 0
