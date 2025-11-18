import {Card, Flex, IconButton, Select, Text, Dialog, Button} from "@radix-ui/themes";
import { RefreshCcw, SaveIcon, SquarePenIcon, TrashIcon } from "lucide-react";
import React, { JSX } from "react";
import type { TimeInterval } from "@/src/types.ts";

interface Props {
    title?: string;
    showTimeInterval?: boolean;
    showSave?: boolean;
    showEdit?: boolean;
    showDelete?: boolean;
    showRefresh?: boolean;

    selectedInterval?: TimeInterval;
    onIntervalChange?: (value: TimeInterval) => void;

    onSaveClick?: () => void;
    onEditClick?: () => void;
    onDeleteClick?: () => void;
    onRefreshClick?: () => void;
}

export default function Header(props: Props): JSX.Element {
    const {
        title,
        showTimeInterval,
        showSave,
        showEdit,
        showDelete,
        showRefresh,
        selectedInterval = "0",
        onIntervalChange,
        onSaveClick,
        onEditClick,
        onDeleteClick,
        onRefreshClick,
    } = props;

    const handleIntervalChange = (value: string) => {
        if (onIntervalChange) {
            onIntervalChange(value as TimeInterval);
        }
    };

    const IntervalSelect = (
        <Select.Root
            value={selectedInterval}
            onValueChange={handleIntervalChange}
        >
            <Select.Trigger variant="soft" style={{ width: "155px" }} />
            <Select.Content variant="soft">
                <Select.Item value="0">This week</Select.Item>
                <Select.Item value="1">This month</Select.Item>
                <Select.Item value="2">This year</Select.Item>
                <Select.Separator />
                <Select.Item value="3">Last week</Select.Item>
                <Select.Item value="4">Last month</Select.Item>
                <Select.Item value="5">Last year</Select.Item>
            </Select.Content>
        </Select.Root>
    );

    return (
        <Card style={{ flex: 1, minWidth: "100%" }} mb="4">
            <Flex direction="row" style={{ justifyContent: "space-between" }}>
                {title && (
                    <Text size="5">{title}</Text>
                )}

                {!title && showTimeInterval && (
                    <Select.Root
                        value={selectedInterval}
                        onValueChange={handleIntervalChange}
                    >
                        <Select.Trigger variant="soft" style={{ width: "10%" }} />
                        <Select.Content variant="soft">
                            <Select.Item value="0">This week</Select.Item>
                            <Select.Item value="1">This month</Select.Item>
                            <Select.Item value="2">This year</Select.Item>
                            <Select.Separator />
                            <Select.Item value="3">Last week</Select.Item>
                            <Select.Item value="4">Last month</Select.Item>
                            <Select.Item value="5">Last year</Select.Item>
                        </Select.Content>
                    </Select.Root>
                )}

                <Flex direction="row" gap="4" wrap="wrap" align="center">
                    {title && showTimeInterval && IntervalSelect}

                    {showSave && (
                        <IconButton
                            variant="soft"
                            onClick={onSaveClick}
                        >
                            <SaveIcon size="20" />
                        </IconButton>
                    )}
                    {showEdit && (
                        <IconButton
                            variant="soft"
                            onClick={onEditClick}
                        >
                            <SquarePenIcon size="20" />
                        </IconButton>
                    )}
                    {showDelete && (
                        <Dialog.Root>
                            <Dialog.Trigger>
                                <IconButton
                                    variant="soft"
                                >
                                    <TrashIcon size="20" />
                                </IconButton>
                            </Dialog.Trigger>

                            <Dialog.Content maxWidth="450px">
                                <Dialog.Title>Delete</Dialog.Title>
                                <Dialog.Description size="2" mb="4">
                                    Are you sure you want to delete this record?
                                    <br/>You can't undo  this action.
                                </Dialog.Description>

                                <Flex gap="3" mt="4" justify="end">
                                    <Dialog.Close>
                                        <Button variant="soft" color="gray">
                                            Cancel
                                        </Button>
                                    </Dialog.Close>
                                    <Dialog.Close>
                                        <Button onClick={onDeleteClick} color="red">Delete</Button>
                                    </Dialog.Close>
                                </Flex>
                            </Dialog.Content>
                        </Dialog.Root>
                    )}
                    {showRefresh && (
                        <IconButton
                            variant="soft"
                            onClick={onRefreshClick}
                        >
                            <RefreshCcw size="20" />
                        </IconButton>
                    )}
                </Flex>
            </Flex>
        </Card>
    );
}