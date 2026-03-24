type FormattedDateProps = {
  value: string;
};

export default function FormattedDate({ value }: FormattedDateProps) {
  const date = new Date(value);

  return <span>{date.toLocaleString()}</span>;
}
