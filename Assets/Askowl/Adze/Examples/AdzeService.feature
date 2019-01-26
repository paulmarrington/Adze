@CustomAsset
#noinspection CucumberUndefinedStep
Feature: Adze
  Adze provides a decoupled layer to external advertising services.

  Rule: First passing service will respond to a display request repeatedly

  Background:
    Given 4 services available
    And they are ordered  as "Round Robin"

  Example: The first service responds
    Given that the first service works
    When I ask for an advertisement
    Then I get the first service
    When I ask for an advertisement again
    Then I get the first service again
