Feature: Create RRR Feature
	For Registrations transactions
	With Action = Create (0)
	On complete we have RRR objects produced

Background: 
	Given We have Parcel with Uid = "Property 1"
	And Registration transaction "REGO" with No."TRN-001"

@mytag
Scenario: Register rights for each target party 
	Given Transaction No."TRN-001" has party "John Doe" with role "Grantee" 
	And Transaction No."TRN-001" has party "Mark Twen" with role "Grantee"
	And Transaction No."TRN-001" has target property with Uid = "Property 1"
	And Transaction No."TRN-001" target parties reference property with Uid = "Property 1"
	When transaction "TRN-001" is completed
	Then Agains property "Property 1" registered 2 "Occupancy" rights
	And Party "John Doe" have active "Occupancy" rights on "Property 1"
	And Party "Mark Twen" have active "Occupancy" rights on "Property 1"
