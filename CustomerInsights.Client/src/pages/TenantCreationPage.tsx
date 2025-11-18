import {Box, Button, Flex, Card, Text} from "@radix-ui/themes";
import {useState} from "react";
import { Stepper } from "../components/Stepper";
import LightRays from "../components/LightRays";

export default function TenantCreationPage() {
  const [activeStep, setActiveStep] = useState(0);
  const [demoStep, setDemoStep] = useState(0);

  const steps = [
    {label: 'Personal Info'},
    {label: 'Address'},
    {label: 'Payment'},
    {label: 'Confirmation'},
  ];

  const handleNext = () => {
    if (activeStep < steps.length - 1) {
      setActiveStep(activeStep + 1);
    }
  };

  const handleBack = () => {
    if (activeStep > 0) {
      setActiveStep(activeStep - 1);
    }
  };

  const handleReset = () => {
    setActiveStep(0);
  };

  return (
      <div
        style={{
            width: "100%",
            height: "100vh",
            overflow: "hidden",       // Kein Scrollen
            display: "flex",
            justifyContent: "center", // Horizontal zentrieren
            alignItems: "center",     // Vertikal zentrieren
            margin: "-15px"
        }}>

        <Card size="4" mb="6">
          <Flex direction="column" gap="6">
            <Stepper activeStep={activeStep} steps={steps} size="1" variant="soft"/>

            <Box
              style={{
                background: 'var(--gray-a2)',
                borderRadius: 'var(--radius-3)',
                padding: 'var(--space-5)',
                minHeight: '200px',
              }}
            >
              <Text size="5" weight="medium" mb="2">
                Step {activeStep + 1}: {steps[activeStep].label}
              </Text>
              <Text color="gray" size="2" mb="4">
                This is where your step content would go. Each step can contain forms,
                information, or any other content you need.
              </Text>

              {activeStep === steps.length - 1 && (
                <Card variant="surface" style={{background: 'var(--green-a3)', borderColor: 'var(--green-a5)'}}>
                  <Flex gap="2" align="center">
                    <Text size="2" weight="medium" style={{color: 'var(--green-11)'}}>
                      ✓ All steps completed! Ready to submit.
                    </Text>
                  </Flex>
                </Card>
              )}
            </Box>

            <Flex gap="3" justify="between">
              <Button
                onClick={handleBack}
                disabled={activeStep === 0}
                variant="soft"
                color="gray"
                size="2"
              >
                Back
              </Button>

              <Flex gap="2">
                <Button onClick={handleReset} variant="outline" color="gray" size="2">
                  Reset
                </Button>
                {activeStep < steps.length - 1 ? (
                  <Button onClick={handleNext} size="2">
                    Next Step
                  </Button>
                ) : (
                  <Button onClick={() => alert('Form submitted!')} size="2" color="green">
                    Submit
                  </Button>
                )}
              </Flex>
            </Flex>
          </Flex>
        </Card>
    <LightRays
      raysOrigin="top-center"
      raysColor="#00ffff"
      raysSpeed={1.5}
      lightSpread={0.8}
      rayLength={1.2}
      followMouse={true}
      mouseInfluence={0.1}
      noiseAmount={0.1}
      distortion={0.05}
      className="custom-rays"
  />
      </div>
  );
}