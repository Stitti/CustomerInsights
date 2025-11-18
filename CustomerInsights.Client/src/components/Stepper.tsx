import React, { useState, ReactNode } from 'react';
import { Theme, Box, Flex, Text, Button, Card } from '@radix-ui/themes';
import '@radix-ui/themes/styles.css';

interface StepProps {
  label: string;
  children?: ReactNode;
}

interface StepperProps {
  activeStep: number;
  children?: ReactNode;
  steps?: { label: string }[];
  size?: '1' | '2' | '3';
  variant?: 'solid' | 'soft' | 'outline';
  className?: string;
}

const Step: React.FC<StepProps> = ({ label }) => {
  return null;
};

const Stepper: React.FC<StepperProps> = ({
  activeStep = 0,
  children,
  steps: stepsProp,
  size = '2',
  variant = 'soft',
  className = '',
}) => {
  const steps = stepsProp ||
    (children ? React.Children.map(children, (child) => {
      if (React.isValidElement(child) && child.props.label) {
        return { label: child.props.label };
      }
      return null;
    })?.filter(Boolean) : []) || [];

    const sizeMap = {
      '1': { circle: 32, font: '1', icon: 16, gap: '1' },
      '2': { circle: 40, font: '2', icon: 20, gap: '2' },
      '3': { circle: 48, font: '3', icon: 24, gap: '3' },
    };

    const currentSize = sizeMap[size];
    
  const getStepClasses = (index: number) => {
    const isActive = index === activeStep;
    const isCompleted = index < activeStep;

    if (variant === 'solid') {
      return {
        base: isCompleted || isActive ? 'rt-variant-solid' : 'rt-variant-soft',
        opacity: isCompleted || isActive ? '1' : '0.6',
      };
    } else if (variant === 'outline') {
      return {
        base: 'rt-variant-outline',
        opacity: isCompleted || isActive ? '1' : '0.5',
      };
    } else {
      return {
        base: 'rt-variant-soft',
        opacity: isCompleted || isActive ? '1' : '0.5',
      };
    }
  };

  return (
    <Box className={className} style={{ width: '100%' }}>
      <Flex direction="row" align="start" justify="between" gap={currentSize.gap}>
        {steps.map((step: any, index: number) => {
          const isActive = index === activeStep;
          const isCompleted = index < activeStep;
          const classes = getStepClasses(index);

          return (
            <Flex
              key={index}
              direction="column"
              align="center"
              gap="2"
              style={{
                flex: 1,
                position: 'relative',
                minWidth: 0,
              }}
            >
              {/* Connector Line - vor dem Circle für z-index */}
              {index < steps.length - 1 && (
                <Box
                  className={`rt-BaseButton ${classes.base}`}
                  style={{
                    position: 'absolute',
                    top: `${currentSize.circle / 2}px`,
                    left: `calc(50% + ${currentSize.circle / 2 + 8}px)`,
                    right: `calc(-50% + ${currentSize.circle / 2 + 8}px)`,
                    height: '2px',
                    opacity: classes.opacity,
                    transform: 'translateY(-1px)',
                    pointerEvents: 'none',
                  }}
                />
              )}

              {/* Circle */}
              <Flex
                className={`rt-BaseButton ${classes.base}`}
                align="center"
                justify="center"
                style={{
                  width: `${currentSize.circle}px`,
                  height: `${currentSize.circle}px`,
                  borderRadius: '50%',
                  fontWeight: isActive ? '700' : '600',
                  transition: 'all 0.2s cubic-bezier(0.4, 0, 0.2, 1)',
                  cursor: 'default',
                  opacity: classes.opacity,
                  flexShrink: 0,
                  position: 'relative',
                  zIndex: 1,
                }}
              >
                {isCompleted ? (
                  <svg
                    width={currentSize.icon}
                    height={currentSize.icon}
                    viewBox="0 0 24 24"
                    fill="none"
                    style={{ display: 'block' }}
                  >
                    <path
                      d="M5 13l4 4L19 7"
                      stroke="currentColor"
                      strokeWidth="3"
                      strokeLinecap="round"
                      strokeLinejoin="round"
                    />
                  </svg>
                ) : (
                  <Text weight="bold" style={{ lineHeight: 1 }}>
                    {index + 1}
                  </Text>
                )}
              </Flex>

              <Text
                weight={isActive ? 'medium' : 'regular'}
                align="center"
                style={{
                  opacity: isActive || isCompleted ? 1 : 0.7,
                  transition: 'all 0.2s cubic-bezier(0.4, 0, 0.2, 1)',
                  wordBreak: 'break-word',
                  hyphens: 'auto',
                }}
              >
                {step.label}
              </Text>
            </Flex>
          );
        })}
      </Flex>
    </Box>
  );
};

export { Stepper, Step };