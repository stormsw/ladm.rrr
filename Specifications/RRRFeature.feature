Feature: Create RRR Feature
	For Registrations transactions
	With Action = Create (0)
	On complete we have RRR objects produced

Background: 
	Given We have Parcel with Uid = 1 
	And Party "John Doe" with id=1
	And Party "Mark Samuels" with id=2
	And Registration transaction "REGO" with No."TRN-001"

@mytag
Scenario: Register rights for each target party 
	Given I have add target Party with id.1 to transaction "TRN-001"
	And I have add target Party with id.2 to transaction "TRN-001"
	And I have add target Property with uid."1" to transaction "TRN-001"
	When transaction "TRN-001" is completed
	Then "2" "Occupancy" RRR produced
